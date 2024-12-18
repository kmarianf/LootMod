using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Base.UI;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;


//PhoenixPoint.Geoscape.Entities.GeoMission.AddCratesToMissionData(TacMissionData, MapPlotDef, bool) : void @06005B9C
//PhoenixPoint.Geoscape.Entities.GeoMission.GetRandomEquipmentCrate(TacMissionData) : ActorDeployData @06005B9E



namespace LootMod
{
    internal class Loot
    {
        private const int MIN_SPAWN_WEIGHT = 100000;  // set to eg 100000 for degugging to make sure the modified items show up frequently
        public Dictionary<string, List<TacticalItemDef>> NewItems = new Dictionary<string, List<TacticalItemDef>>();
        private static ModMain modInstance;
        private static DefCache defCache;
        private static List<NegativeModification> negativeModifications;
        private static List<PositiveModification> positiveModifications;

        public Loot(ModMain i, DefCache c)
        {
            modInstance = i;
            defCache = c;
            negativeModifications = GetAllModifications<NegativeModification>();
            positiveModifications = GetAllModifications<PositiveModification>();
        }

        public void InitModifiedItems()
        {
            _createModifiedVersionsOfItems();
            // TODO recalc the devCache if I need to find the new items in it
            ModHandler.modInstance.Logger.LogInfo($"Initialized {NewItems.Values.Sum(list => list.Count)} new items");
        }

        /// <summary>
        /// for each class inheriting from T, instanciate them, then return the list of objects
        /// </summary>
        private static List<T> GetAllModifications<T>() where T : BaseModification
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => (T)Activator.CreateInstance(t))
                .ToList();
        }

        private void _createModifiedVersionsOfItems()
        {
            // TODO create modified versions of all items
            List<TacticalItemDef> items = new List<TacticalItemDef> {
                defCache.GetDef<WeaponDef>("PX_AssaultRifle_WeaponDef"),
                defCache.GetDef<WeaponDef>("PX_GrenadeLauncher_WeaponDef")
            };
            items.ForEach(i => NewItems.Add(i.name, _createModifiedVersionsOfItem(i)));
        }

        private List<TacticalItemDef> _createModifiedVersionsOfItem(TacticalItemDef originalItem)
        {
            string originallocalizationName = originalItem.ViewElementDef.DisplayName1.Localize();  // get the original name of the item. the modifications will edit it.
            List<(TacticalItemDef Item, float RelSpawnWeight)> tempNewItems = new List<(TacticalItemDef, float)>();
            var combos1Neg1Pos = from negativeModification in negativeModifications
                                 from positiveModification in positiveModifications
                                 select new List<BaseModification> { negativeModification, positiveModification };
            // create one new item for each combo
            foreach (List<BaseModification> combo in combos1Neg1Pos)
            {
                // validate that the modifications combination are valid and applicable to this item. skip this combo if it isnt valid.
                if (combo.Any(m => m.IsModificationOrComboInvalid(originalItem, combo))) continue;

                string comboName = string.Join("_", combo.Select(mod => mod.Name).ToArray());
                TacticalItemDef newItem = _createBaseCopy(originalItem, comboName);
                float relativeSpawnWeight = 1f;  // will be multiplied with each modifications SpawnWeightMultiplier, and then later adjusted to the original spawn weight
                List<string> localizationDesc = new List<string>();
                string localizationName = originallocalizationName;
                foreach (BaseModification modification in combo)
                {
                    modification.AddModification(newItem);
                    relativeSpawnWeight *= modification.SpawnWeightMultiplier;
                    localizationName = modification.EditLocalozationName(localizationName);
                    localizationDesc.Add(modification.GetLocalozationDesc());
                }
                ModHandler.LocalizationHandler.AddLine(newItem.ViewElementDef.DisplayName1.LocalizationKey, localizationName);
                localizationDesc.Reverse(); // make the order of the descriptions match the order of the name prefixes
                ModHandler.LocalizationHandler.AddLine(newItem.ViewElementDef.Description.LocalizationKey, string.Join("\n", localizationDesc));
                tempNewItems.Add((newItem, relativeSpawnWeight));
            }

            // adjust spawn weights
            int baseSpawnWeight = originalItem.CrateSpawnWeight;
            if (baseSpawnWeight < MIN_SPAWN_WEIGHT) baseSpawnWeight = MIN_SPAWN_WEIGHT;  // everything can be found with at least a small chance.
            // TODO with a small spawn weight and a high number of modified versions, the smallestSpawnWeightUnit will be <<1,
            // so even after multiplying with the RelSpawnWeight of each version, it will be below 1, and then rounded up to 1.
            // this means all items will have a spawn weight of 1 regardless of their SpawnWeightMultiplier, and in total the versions of the item will have a spawn weight >> 10
            float smallestSpawnWeightUnit = baseSpawnWeight / tempNewItems.Sum(entry => entry.RelSpawnWeight);
            foreach (var entry in tempNewItems)
            {
                entry.Item.CrateSpawnWeight = (int)Math.Ceiling(entry.RelSpawnWeight * smallestSpawnWeightUnit);
            }

            return tempNewItems.Select(entry => entry.Item).ToList();
        }

        /// <summary>
        /// creates a deep copy with only the various names and IDs changed to match the modified version of the weapon.
        /// </summary>
        private TacticalItemDef _createBaseCopy(TacticalItemDef originalItem, string modificationName)
        {
            WeaponDef newWeapon = defCache.Repo.CreateDef<WeaponDef>($"LOOT_ID_{originalItem.name}_{modificationName}", originalItem);
            newWeapon.name = $"LOOT_NAME_{originalItem.name}_{modificationName}";
            newWeapon.IsPickable = true;  // TODO dnspy this

            // copy the ViewElementDef, change the names and IDs
            ViewElementDef ved = defCache.Repo.CreateDef<ViewElementDef>($"LOOT_ID_VED_{originalItem.name}_{modificationName}", originalItem.ViewElementDef);
            Helper.CopyFieldsByReflection(originalItem.ViewElementDef, ved);
            ved.name = $"E_View [{newWeapon.name}_{modificationName}]";
            ved.Name = $"LOOT_VED_NAME_{newWeapon.name}_{modificationName}";
            ved.DisplayName1 = new LocalizedTextBind($"LOC_NAME_LOOT_{originalItem.name}_{modificationName}");
            ved.Description = new LocalizedTextBind($"LOC_DESC_LOOT_{originalItem.name}_{modificationName}");
            newWeapon.ViewElementDef = ved;

            // create a new DamagePayload and copy all the values if this is a weapons.
            if (originalItem is WeaponDef originalWeapon)
            {
                if (originalWeapon.DamagePayload == null)
                {
                    ModHandler.modInstance.Logger.LogInfo($"Found a weapon with no DamagePayload: {originalWeapon.name}");
                }
                else
                {
                    DamagePayload newDamagePayload = new DamagePayload();
                    Helper.CopyFieldsByReflection(originalWeapon.DamagePayload, newDamagePayload);
                    newWeapon.DamagePayload = newDamagePayload;

                    // also copy all DamageKeywords.
                    if (newWeapon.DamagePayload.DamageKeywords == null)
                    {
                        ModHandler.modInstance.Logger.LogInfo($"Found a weapon with no DamageKeywords: {originalWeapon.name}");
                    }
                    else
                    {
                        List<DamageKeywordPair> newDamageKeywords = new List<DamageKeywordPair>();
                        foreach (DamageKeywordPair originalDamageKeywordPair in originalWeapon.DamagePayload.DamageKeywords)
                        {
                            DamageKeywordPair newDamageKeywordPair = new DamageKeywordPair();
                            Helper.CopyFieldsByReflection(originalDamageKeywordPair, newDamageKeywordPair);
                            newDamageKeywords.Add(newDamageKeywordPair);

                        }
                        newDamagePayload.DamageKeywords = newDamageKeywords;
                    }
                }
            }
            return newWeapon;
        }
    }
}
