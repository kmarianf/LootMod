using System;
using System.Linq;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace LootMod
{
    internal class ModHandler
    {
        // TODO get the repo and do the changes after TftV has made their changes, eg by doing everything in the geoscape load (and tactical load? or version text load?)
        internal static readonly DefRepository repo = GameUtl.GameComponent<DefRepository>();
        internal static HarmonyLib.Harmony harmony;
        internal static ModMain modInstance;
        public static Loot Loot;
        internal static LocalizationHandler LocalizationHandler;
        public const int SPAWN_WEIGHT_MULTIPLIER = 100;


        public ModHandler(ModMain instance, HarmonyLib.Harmony h)
        {
            modInstance = instance;
            LocalizationHandler = new LocalizationHandler();
            harmony = h;
        }

        public void ApplyChanges()
        {
            WeaponDef ks_obliterator = DefCache.GetDef<WeaponDef>("KS_Obliterator_WeaponDef");
            modInstance.Logger.LogInfo($"Obliterator ChargesMax: {ks_obliterator.ChargesMax}. if this is 0, TftV isnt loaded yet!");

            try
            {
                Helper.DeleteFile();
                Exploration();
                IncreaseAllSpawnWeights();
                Loot = new Loot(modInstance);
                Loot.InitModifiedItems();
            }
            catch (Exception e)
            {
                modInstance.Logger.LogInfo($"Error:\n{e.Message}\n{e.StackTrace}");
            }


            LocalizationHandler.AddLocalizationFromCSV();
            harmony.PatchAll();
        }

        /// <summary>
        /// CrateSpawnWeight is an int. Increase the CrateSpawnWeight of all items to allow more granular changes, 
        /// </summary>
        private static void IncreaseAllSpawnWeights()
        {
            foreach (var item in DefCache.Repo.GetAllDefs<ItemDef>())
            {
                item.CrateSpawnWeight *= SPAWN_WEIGHT_MULTIPLIER;
            }
        }

        public void Exploration()
        {
            modInstance.Logger.LogInfo($"Loot mod exploration...");
            Helper.AppendToFile("");


            Helper.AppendToFile("\nAbilities per item:");
            foreach (var item in DefCache.Repo.GetAllDefs<TacticalItemDef>())
            {
                if (item.Abilities.Any() && !(item is WeaponDef))
                {
                    Helper.AppendToFile($"{item.name}: {string.Join(", ", item.Abilities.Select(i => i.name))}");
                }
            }


            //ApplyStatusAbilityDef regenerationAbility = DefCache.GetDef<ApplyStatusAbilityDef>("Regeneration_Torso_Passive_AbilityDef");
            //Helper.AppendToFile($"locs of Regeneration_Torso_Passive_AbilityDef");
            //Helper.AppendToFile($"regenerationAbility.ViewElementDef.name: {regenerationAbility.ViewElementDef.name}");
            //Helper.AppendToFile($"regenerationAbility.ViewElementDef.Name: {regenerationAbility.ViewElementDef.Name}");
            //Helper.AppendToFile($"{regenerationAbility.ViewElementDef.Description.LocalizationKey} - {regenerationAbility.ViewElementDef.Description.Localize()}");
            //Helper.AppendToFile($"{regenerationAbility.ViewElementDef.DisplayName1.LocalizationKey} - {regenerationAbility.ViewElementDef.DisplayName1.Localize()}");
            //Helper.AppendToFile($"{regenerationAbility.ViewElementDef.DisplayName2.LocalizationKey} - {regenerationAbility.ViewElementDef.DisplayName2.Localize()}");
            //Helper.AppendToFile($"regenerationAbility.StatusDef:");
            //Helper.PrintPropertiesAndFields(regenerationAbility.StatusDef, modInstance, "- ");
            //if (regenerationAbility.StatusDef is HealthChangeStatusDef healthChangeStatusDef)
            //{
            //    Helper.AppendToFile($"healthChangeStatusDef.Visuals:");
            //    Helper.AppendToFile($"healthChangeStatusDef.Visuals.name: {healthChangeStatusDef.Visuals.name}");
            //    Helper.AppendToFile($"healthChangeStatusDef.Visuals.Name: {healthChangeStatusDef.Visuals.Name}");
            //    Helper.AppendToFile($"healthChangeStatusDef.Visuals.Description.LocalizationKey: {healthChangeStatusDef.Visuals.Description.LocalizationKey} - {healthChangeStatusDef.Visuals.Description.Localize()}");
            //    Helper.AppendToFile($"healthChangeStatusDef.Visuals.DisplayName1.LocalizationKey: {healthChangeStatusDef.Visuals.DisplayName1.LocalizationKey} - {healthChangeStatusDef.Visuals.DisplayName1.Localize()}");
            //    Helper.AppendToFile($"healthChangeStatusDef.Visuals.DisplayName2.LocalizationKey: {healthChangeStatusDef.Visuals.DisplayName2.LocalizationKey} - {healthChangeStatusDef.Visuals.DisplayName2.Localize()}");
            //}

            //foreach (TacticalItemDef item in DefCache.Repo.GetAllDefs<TacticalItemDef>())
            //{
            //    if (item.RequiredSlotBinds == null || item.RequiredSlotBinds.Length <= 0)
            //    {
            //        Helper.AppendToFile($"{item.name} has no RequiredSlotBinds");
            //    }
            //    if (item.RequiredSlotBinds.Length >= 2)
            //    {
            //        Helper.AppendToFile($"{item.name} has {item.RequiredSlotBinds.Length} RequiredSlotBinds");
            //    }
            //}
            //Helper.AppendToFile("");
            //Helper.AppendToFile("");


            //foreach (var name in new List<string>() { "PX_Assault_Helmet_BodyPartDef", "PX_Assault_Legs_ItemDef", "PX_Assault_Torso_BodyPartDef", "AN_Assault_Helmet_BodyPartDef" })
            //{
            //    TacticalItemDef item = (TacticalItemDef)DefCache.GetDef(name);
            //    if (item == null)
            //    {
            //        Helper.AppendToFile("couldnt find item");
            //        continue;
            //    }
            //    Helper.AppendToFile($"item {item.name}");
            //    Helper.PrintPropertiesAndFields(item, modInstance, "  - ");
            //    Helper.AppendToFile($"- item {item.BodyPartAspectDef}");
            //    Helper.PrintPropertiesAndFields(item.BodyPartAspectDef, modInstance, "  - ");
            //    Helper.AppendToFile($"- item ({item.RequiredSlotBinds.Length}) {item.RequiredSlotBinds}");
            //    foreach (var slot in item.RequiredSlotBinds)
            //    {
            //        Helper.PrintPropertiesAndFields(slot, modInstance, "  - ");
            //        Helper.PrintPropertiesAndFields(slot.RequiredSlot, modInstance, "    - ");
            //    }

            //    Helper.AppendToFile("");
            //}



            // TacMissionTypeDef
            //Helper.AppendToFile("TacMissionTypeDefs with MissionSpecificCrates");
            //foreach (var missionType in DefCache.Repo.GetAllDefs<TacMissionTypeDef>())
            //{
            //    if (missionType.MissionSpecificCrates != null) Helper.AppendToFile($"{missionType.name}");
            //}


            // TacticalAbilityDef
            //modInstance.Logger.LogInfo($"printing all regen TacticalAbilityDefs");
            //foreach (var ability in DefCache.Repo.GetAllDefs<TacticalAbilityDef>())
            //{
            //    Helper.AppendToFile($"{ability.name}: {ability.name}");
            //    if (ability.name.ToLower().Contains("regen"))
            //    {
            //        Helper.AppendToFile($"name: {ability.name}");
            //        Helper.PrintPropertiesAndFields(ability, modInstance, "- ");
            //        if (ability.TargetingDataDef != null)
            //        {
            //            Helper.AppendToFile($"- ability.TargetingDataDef:");
            //            Helper.PrintPropertiesAndFields(ability.TargetingDataDef, modInstance, "  - ");
            //            Helper.PrintPropertiesAndFields(ability.TargetingDataDef.Target, modInstance, "    - ");
            //        }
            //        if (ability.AbilitiesRequired != null)
            //        {
            //            Helper.AppendToFile($"- ability.AbilitiesRequired ({ability.AbilitiesRequired.Length}):");
            //            foreach (var req in ability.AbilitiesRequired)
            //            {
            //                Helper.AppendToFile($"  - {req.name}");
            //            }
            //        }
            //        if (ability is ApplyStatusAbilityDef statusAbility && statusAbility.TargetApplicationConditions != null)
            //        {
            //            Helper.AppendToFile($"- ability.TargetApplicationConditions ({statusAbility.TargetApplicationConditions.Length}):");
            //            foreach (var con in statusAbility.TargetApplicationConditions)
            //            {
            //                Helper.PrintPropertiesAndFields(con, modInstance, "  - ");
            //            }
            //            Helper.AppendToFile($"- ability.StatusDef:");
            //            Helper.PrintPropertiesAndFields(statusAbility.StatusDef, modInstance, "  - ");
            //            if (statusAbility.StatusDef is HealthChangeStatusDef healthChangeStatusDef && healthChangeStatusDef.BodypartSlotNames != null)
            //            {
            //                Helper.AppendToFile($"  - ability.StatusDef.BodypartSlotNames ({healthChangeStatusDef.BodypartSlotNames.Length}):");
            //                foreach (var slot in healthChangeStatusDef.BodypartSlotNames)
            //                {
            //                    Helper.AppendToFile($"    - {slot}");
            //                }
            //            }
            //        }
            //        if (ability.EventOnActivate != null)
            //        {
            //            Helper.AppendToFile($"- ability.EventOnActivate:");
            //            Helper.PrintPropertiesAndFields(ability.EventOnActivate, modInstance, "  - ");
            //        }
            //        Helper.AppendToFile("");
            //    }
            //}






            // range exploration
            //foreach (var weapon in defCache.Repo.GetAllDefs<WeaponDef>())
            //{
            //    Helper.AppendToFile($"exploring {weapon.name}");
            //    Helper.AppendToFile($"weapon.DamagePayload.DamageDeliveryType: {weapon.DamagePayload.DamageDeliveryType}");
            //    Helper.AppendToFile($"weapon.AreaRadius (calced): {weapon.AreaRadius}");
            //    Helper.AppendToFile($"weapon.SpreadRadius: {weapon.SpreadRadius}");
            //    Helper.AppendToFile($"weapon.SpreadDegrees: {weapon.SpreadDegrees}");
            //    Helper.AppendToFile($"weapon.MaximumRange (calced): {weapon.MaximumRange}");
            //    Helper.AppendToFile($"weapon.EffectiveRange (calced): {weapon.EffectiveRange}");
            //    Helper.AppendToFile($"weapon.DamagePayload.Range: {weapon.DamagePayload.Range}");
            //    Helper.AppendToFile($"weapon.DamagePayload.AoeRadius: {weapon.DamagePayload.AoeRadius}");
            //    Helper.AppendToFile($"weapon.DamagePayload.ConeRadius: {weapon.DamagePayload.ConeRadius}");
            //    Helper.AppendToFile($"weapon.DamagePayload.GenerateRangeValue (calced): {weapon.DamagePayload.GenerateRangeValue()}");
            //    Helper.AppendToFile("");
            //}

            //foreach (var name in new List<string>() { "LOOT_NAME_PX_Pistol_WeaponDef_AddViral_NegativeWeight", "PX_Pistol_WeaponDef" })
            //TacticalItemDef item = (TacticalItemDef)defCache.GetDef(name);
            //if (item == null) Helper.AppendToFile("couldnt find item");
            //Helper.PrintPropertiesAndFields(item, modInstance);



            //Helper.AppendToFile("weapon.DamagePayload + DamageKeywords:");
            //if (item is WeaponDef weapon)
            //{
            //    Helper.AppendToFile("weapon.DamagePayload:");
            //    Helper.PrintPropertiesAndFields(weapon.DamagePayload, modInstance);
            //    Helper.AppendToFile("");
            //    foreach (var pair in weapon.DamagePayload.DamageKeywords)
            //    {
            //        Helper.AppendToFile("");
            //        Helper.AppendToFile($"keyword {pair.DamageKeywordDef}, value {pair.Value}");
            //        Helper.PrintPropertiesAndFields(pair.DamageKeywordDef, modInstance);
            //    }
            //}
            //else Helper.AppendToFile($"{name} is not a WeaponDef");
            //Helper.AppendToFile("");



            //Helper.AppendToFile($"Name;CrateSpawnWeight;traits;Abilities");
            //foreach (TacticalItemDef item in defCache.Repo.GetAllDefs<TacticalItemDef>())
            //{
            //    Helper.AppendToFile($"{item.name};{item.CrateSpawnWeight};{string.Join(", ", item.Traits)};{string.Join(", ", item.Abilities.Select(ability => ability.name))}");
            //}

            //Helper.AppendToFile($"{item.Abilities.Count()} item.Abilities:");
            //foreach (var ability in item.Abilities)
            //{
            //    Helper.PrintPropertiesAndFields(ability, modInstance);
            //    Helper.AppendToFile("");
            //}




            //Helper.AppendToFile("does PX_AssaultRifle_WeaponDef have a BodyPartAspectDef?");
            //TacticalItemDef item2 = (TacticalItemDef)defCache.GetDef("PX_AssaultRifle_WeaponDef");
            //if (item2.BodyPartAspectDef != null)
            //{
            //    Helper.PrintPropertiesAndFields(item2.BodyPartAspectDef, modInstance);
            //}
            //else
            //{
            //    Helper.AppendToFile("PX_AssaultRifle_WeaponDef has no BodyPartAspectDef");
            //}
            //Helper.AppendToFile("");



            //Helper.AppendToFile($"DisplayName1 = {item.ViewElementDef.DisplayName1.LocalizationKey} - {item.ViewElementDef.DisplayName1.Localize()} ");
            //Helper.AppendToFile($"DisplayName2 = {item.ViewElementDef.DisplayName2.LocalizationKey} - {item.ViewElementDef.DisplayName2.Localize()} ");
            //Helper.AppendToFile($"Description = {item.ViewElementDef.Description.LocalizationKey} - {item.ViewElementDef.Description.Localize()} ");


            //Helper.PrintPropertiesAndFields(origAres.DamagePayload, modInstance);
            //Helper.AppendToFile("");
            //foreach (DamageKeywordPair d in origAres.DamagePayload.DamageKeywords)
            //{
            //    Helper.PrintPropertiesAndFields(d, modInstance);
            //    Helper.AppendToFile("");
            //    Helper.PrintPropertiesAndFields(d.DamageKeywordDef, modInstance);
            //    Helper.AppendToFile("");
            //modInstance.Logger.LogInfo($"");


            //modInstance.Logger.LogInfo($"printing weapon damage keywords to file");
            //Helper.AppendToFile("\n --- weapon damage keywords --- \n");
            //foreach (WeaponDef w in defCache.Repo.GetAllDefs<WeaponDef>())
            //{
            //    List<string> l = new List<string>();
            //    foreach (DamageKeywordPair pair in w.DamagePayload.DamageKeywords)
            //    {
            //        l.Add($"{pair.DamageKeywordDef}: {pair.Value}");
            //    }
            //    Helper.AppendToFile($"{w.name} - {string.Join(", ", l)}");
            //}


        }
    }
}
