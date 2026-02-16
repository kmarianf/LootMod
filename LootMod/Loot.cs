using Base.UI;
using LootMod.Modifications;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//PhoenixPoint.Geoscape.Entities.GeoMission.AddCratesToMissionData(TacMissionData, MapPlotDef, bool) : void @06005B9C
//PhoenixPoint.Geoscape.Entities.GeoMission.GetRandomEquipmentCrate(TacMissionData) : ActorDeployData @06005B9E



namespace LootMod
{
    internal class Loot
    {
        private const int MIN_SPAWN_WEIGHT = ModHandler.SPAWN_WEIGHT_MULTIPLIER * 50;
        private const int WEAPON_SPAWN_WEIGHT_MULTIPLIER = 4;
        private const int ARMOR_SPAWN_WEIGHT_MULTIPLIER = 2;
        public Dictionary<string, List<TacticalItemDef>> NewItems = new Dictionary<string, List<TacticalItemDef>>();
        private static ModMain modInstance;
        private static List<NegativeModification> negativeModifications;
        private static List<PositiveModification> positiveModifications;
        private static bool csvHeaderWritten = false;

        public Loot(ModMain i)
        {
            modInstance = i;
            negativeModifications = GetAllModifications<NegativeModification>();
            positiveModifications = GetAllModifications<PositiveModification>();
        }

        public void InitModifiedItems()
        {
            _createModifiedVersionsOfItems();
            modInstance.Logger.LogInfo($"Initialized {NewItems.Values.Sum(list => list.Count)} new items");
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
            // create modified versions of all items that make sense to find in missions
            List<TacticalItemDef> items = ItemsToModify.Items.Select(i => (TacticalItemDef)DefCache.GetDef(i)).ToList();
            //List < TacticalItemDef > items = new List<TacticalItemDef> {
            //(TacticalItemDef)defCache.GetDef("PX_AssaultRifle_WeaponDef"),
            //};
            items.ForEach(i => NewItems.Add(i.name, _createModifiedVersionsOfItem(i)));
        }

        // deprecated, couldnt find a logic that actually finds exactly those items that make sense to modfiy. hardcode the list instead :(
        private List<TacticalItemDef> getItemsToModify()
        {
            List<TacticalItemDef> itemsToModify = new List<TacticalItemDef>();
            List<string> validItemNamePrefixes = new List<string> { "PX", "AN", "KS", "NJ", "SY" };
            // exlcude various items
            foreach (TacticalItemDef item in DefCache.Repo.GetAllDefs<TacticalItemDef>())
            {
                // items with CrateSpawnWeight of 0 seem to be the ones that shouldnt be found, eg mutations, vehicle items and NJ_TobiasWestGun_WeaponDef
                if (item.CrateSpawnWeight == 0) continue;
                // items with CrateSpawnWeight of 1 seem to be the ones that musnt be found, eg humand and alien body parts
                if (item.CrateSpawnWeight == 1) continue;
                // TODO do include eg KS_Devastator_WeaponDef
                if (!validItemNamePrefixes.Any(prefix => item.name.StartsWith(prefix))) continue;  // exclude items that do not have any of these prefixes
                itemsToModify.Add(item);
            }
            return itemsToModify;
        }

        private List<TacticalItemDef> _createModifiedVersionsOfItem(TacticalItemDef originalItem)
        {
            string originallocalizationName = _getCorrectLocalizationName(originalItem);  // get the original name of the item. the modifications will edit it.
            List<(TacticalItemDef Item, float RelSpawnWeight)> tempNewItems = new List<(TacticalItemDef, float)>();

            //var combos1Neg1Pos = from negativeModification in negativeModifications
            //                     from positiveModification in positiveModifications
            //                     select new List<BaseModification> { positiveModification, negativeModification };

            var combos2Pos1Neg = from negativeModification in negativeModifications
                                 from positiveMod1 in positiveModifications
                                 from positiveMod2 in positiveModifications
                                 where positiveMod1 != positiveMod2
                                 select new List<BaseModification> { positiveMod1, positiveMod2, negativeModification }
                                 into combo
                                 orderby combo.OfType<PositiveModification>().First().GetType().Name, combo.OfType<PositiveModification>().Last().GetType().Name
                                 select combo;

            //var allCombos = combos1Neg1Pos.Concat(combos2Pos1Neg);
            var allCombos = combos2Pos1Neg;

            // create one new item for each combo
            foreach (List<BaseModification> combo in allCombos)
            {
                try
                {
                    // validate that the modifications combination are valid and applicable to this item. skip this combo if it isnt valid.
                    if (combo.Any(m => m.IsModificationOrComboInvalid(originalItem, combo))) continue;

                    string comboId = string.Join("_", combo.Select(mod => mod.modificationId).ToArray());
                    TacticalItemDef newItem = _createBaseCopy(originalItem, comboId);
                    float relativeSpawnWeight = 1f;  // will be multiplied with each modifications SpawnWeightMultiplier, and then later adjusted to the original spawn weight
                    List<string> localizationDesc = new List<string>();
                    string localizationName = originallocalizationName;
                    foreach (BaseModification modification in combo)
                    {
                        modification.ApplyModification(newItem);
                        relativeSpawnWeight *= modification.SpawnWeightMultiplier;
                        localizationName = modification.EditLocalozationName(localizationName);
                        localizationDesc.Add(modification.GetLocalizationDesc());
                    }
                    ModHandler.LocalizationHandler.AddLine(newItem.ViewElementDef.DisplayName1.LocalizationKey, localizationName);
                    localizationDesc.Reverse(); // make the order of the descriptions match the order of the name prefixes
                    ModHandler.LocalizationHandler.AddLine(newItem.ViewElementDef.Description.LocalizationKey, string.Join(". ", localizationDesc) + ".");
                    tempNewItems.Add((newItem, relativeSpawnWeight));
                }
                catch (Exception ex)
                {
                    string comboNames = string.Join(", ", combo.Select(m => $"{m.Name} ({m.modificationId})"));
                    modInstance.Logger.LogInfo($"Error when modifying {originallocalizationName} with combo [{comboNames}]. Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                    throw;
                }
            }

            // adjust spawn weights
            int baseSpawnWeight = originalItem.CrateSpawnWeight;
            if (baseSpawnWeight < MIN_SPAWN_WEIGHT) baseSpawnWeight = MIN_SPAWN_WEIGHT;  // everything can be found with at least a small chance.
            // make weapons and armor less rare
            if (originalItem is WeaponDef) baseSpawnWeight *= WEAPON_SPAWN_WEIGHT_MULTIPLIER;
            else if (originalItem.name.Contains("BodyPartDef")) baseSpawnWeight *= ARMOR_SPAWN_WEIGHT_MULTIPLIER;
            float smallestSpawnWeightUnit = baseSpawnWeight / tempNewItems.Sum(entry => entry.RelSpawnWeight);
            foreach (var entry in tempNewItems)
            {
                entry.Item.CrateSpawnWeight = (int)Math.Ceiling(entry.RelSpawnWeight * smallestSpawnWeightUnit);
            }

            // Log all created modified items so we can inspect what got created
            if (!csvHeaderWritten)
            {
                Helper.AppendToFile("CreatedName,OriginalName,RelSpawnWeight,CrateSpawnWeight", "LootModLog.csv");
                csvHeaderWritten = true;
            }

            foreach (var entry in tempNewItems)
            {
                try
                {
                    string createdName = entry.Item.name;
                    string origName = originalItem.name;
                    float rel = entry.RelSpawnWeight;
                    int finalWeight = entry.Item.CrateSpawnWeight;
                    Helper.AppendToFile($"{_escapeCsv(createdName)},{_escapeCsv(origName)},{rel},{finalWeight}", "LootModLog.csv");
                }
                catch (Exception ex)
                {
                    modInstance.Logger.LogInfo($"Error while logging created item for {originalItem.name}: {ex.Message}");
                }
            }

            return tempNewItems.Select(entry => entry.Item).ToList();
        }

        /// <summary>
        /// some items, eg armors, just have eg "LEGS" as DisplayName1, and DisplayName2 is the correct one.
        /// weapons seem to use DisplayName1m, and DisplayName2 is empty.
        /// current logic: pick the longer one until I know better
        /// </summary>
        private string _getCorrectLocalizationName(TacticalItemDef item)
        {
            string name1 = item.ViewElementDef.DisplayName1.Localize();
            string name2 = item.ViewElementDef.DisplayName2.Localize();
            //if (name1 != name2) Helper.AppendToFile($"item {item.name}: DisplayName1 = {name1}, DisplayName2 = {name2} ---------------------------------");
            //else Helper.AppendToFile($"item {item.name}: DisplayName1/DisplayName2 = {name1}");
            if (name1.Length > name2.Length) return name1;
            else return name2;
        }

        /// <summary>
        /// creates a deep copy with only the various names and IDs changed to match the modified version of the weapon.
        /// </summary>
        private TacticalItemDef _createBaseCopy(TacticalItemDef originalItem, string modificationId)
        {
            TacticalItemDef newItem = (TacticalItemDef)DefCache.Repo.CreateDef($"LOOT_ID_{originalItem.name}_{modificationId}", originalItem);
            newItem.name = $"LOOT_NAME_{originalItem.name}_{modificationId}";
            newItem.IsPickable = true;  // TODO dnspy this
            newItem.DropOnActorDeath = true;  // make sure they can be dropped (though there is still the TacticalItemDef.DestroyOnActorDeathPerc)

            // copy the ViewElementDef, change the names and IDs
            ViewElementDef ved = DefCache.Repo.CreateDef<ViewElementDef>($"LOOT_ID_VED_{originalItem.name}_{modificationId}", originalItem.ViewElementDef);
            Helper.CopyFieldsByReflection(originalItem.ViewElementDef, ved);
            ved.name = $"E_View [{newItem.name}]";
            ved.Name = $"LOOT_VED_NAME_{originalItem.name}_{modificationId}";
            // some items use DisplayName1, some use DisplayName2. I havent found any that use both for different purposes, so I just override both.
            ved.DisplayName1 = new LocalizedTextBind($"LOC_NAME_LOOT_{originalItem.name}_{modificationId}");
            ved.DisplayName2 = new LocalizedTextBind($"LOC_NAME_LOOT_{originalItem.name}_{modificationId}");
            ved.Description = new LocalizedTextBind($"LOC_DESC_LOOT_{originalItem.name}_{modificationId}");
            newItem.ViewElementDef = ved;

            // also copy BodyPartAspectDef if the original has one. eg armors seem to have one, but weapons have none.
            if (originalItem.BodyPartAspectDef != null)
            {
                BodyPartAspectDef bpad = DefCache.Repo.CreateDef<BodyPartAspectDef>($"LOOT_ID_BPAD_{originalItem.name}_{modificationId}", originalItem.BodyPartAspectDef);
                newItem.BodyPartAspectDef = bpad;
            }

            // create a new DamagePayload and copy all the values if this is a weapon.
            if (originalItem is WeaponDef originalWeapon)
            {
                WeaponDef newWeapon = (WeaponDef)newItem;  // CreateDef copies the type of the original, so if originalItem is a WeaponDef, newWeapon is must also be a WeaponDef 
                if (originalWeapon.DamagePayload == null)
                {
                    modInstance.Logger.LogInfo($"Found a weapon with no DamagePayload: {originalWeapon.name}");
                }
                else
                {
                    DamagePayload newDamagePayload = new DamagePayload();
                    Helper.CopyFieldsByReflection(originalWeapon.DamagePayload, newDamagePayload);
                    newWeapon.DamagePayload = newDamagePayload;

                    // also copy all DamageKeywords.
                    if (newWeapon.DamagePayload.DamageKeywords == null)
                    {
                        modInstance.Logger.LogInfo($"Found a weapon with no DamageKeywords: {originalWeapon.name}");
                    }
                    else
                    {
                        List<DamageKeywordPair> newDamageKeywords = new List<DamageKeywordPair>();
                        foreach (DamageKeywordPair originalDamageKeywordPair in originalWeapon.DamagePayload.DamageKeywords)
                        {
                            DamageKeywordPair newDamageKeywordPair = new DamageKeywordPair()
                            {
                                DamageKeywordDef = originalDamageKeywordPair.DamageKeywordDef,
                                Value = originalDamageKeywordPair.Value
                            };
                            newDamageKeywords.Add(newDamageKeywordPair);

                        }
                        newDamagePayload.DamageKeywords = newDamageKeywords;
                    }
                }
            }
            return newItem;
        }

        private static string _escapeCsv(string input)
        {
            if (input == null) return "";
            bool mustQuote = input.Contains(",") || input.Contains("\"") || input.Contains("\n") || input.Contains("\r");
            string escaped = input.Replace("\"", "\"\"");
            if (mustQuote) return "\"" + escaped + "\"";
            return escaped;
        }
    }
}
