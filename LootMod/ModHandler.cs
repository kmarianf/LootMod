using System;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace LootMod
{
    internal class ModHandler
    {
        // TODO get the repo and do the changes after TftV has made their changes, eg by doing everything in the geoscape load (and tactical load? or version text load?)
        internal static readonly DefRepository repo = GameUtl.GameComponent<DefRepository>();
        internal static HarmonyLib.Harmony harmony;
        internal static ModMain modInstance;
        public DefCache defCache = new DefCache();
        public static Loot Loot;
        internal static LocalizationHandler LocalizationHandler;


        public ModHandler(ModMain instance, HarmonyLib.Harmony h)
        {
            modInstance = instance;
            LocalizationHandler = new LocalizationHandler();
            harmony = h;
        }

        public void ApplyChanges()
        {
            //WeaponDef ks_obliterator = (WeaponDef)Repo.GetDef("7e7ea9c9-e939-dc14-8a23-3a749e76cd98"); // KS_Obliterator_WeaponDef
            WeaponDef ks_obliterator = defCache.GetDef<WeaponDef>("KS_Obliterator_WeaponDef");
            modInstance.Logger.LogInfo($"Obliterator ChargesMax: {ks_obliterator.ChargesMax}. if this is 0, TftV isnt loaded yet!");

            try
            {
                Helper.DeleteFile();
                Loot = new Loot(modInstance, defCache);
                Loot.InitModifiedItems();
                defCache = new DefCache();
                Exploration();
            }
            catch (Exception e)
            {
                modInstance.Logger.LogInfo($"Error:\n{e.Message}\n{e.StackTrace}");
            }


            LocalizationHandler.AddLocalizationFromCSV();
            harmony.PatchAll();
        }

        public void Exploration()
        {
            modInstance.Logger.LogInfo($"Loot mod exploration...");
            Helper.AppendToFile("");

            foreach (var weapon in defCache.Repo.GetAllDefs<WeaponDef>())
            {
                Helper.AppendToFile($"exploring {weapon.name}");
                Helper.AppendToFile($"weapon.DamagePayload.DamageDeliveryType: {weapon.DamagePayload.DamageDeliveryType}");
                Helper.AppendToFile($"weapon.AreaRadius (calced): {weapon.AreaRadius}");
                Helper.AppendToFile($"weapon.SpreadRadius: {weapon.SpreadRadius}");
                Helper.AppendToFile($"weapon.SpreadDegrees: {weapon.SpreadDegrees}");
                Helper.AppendToFile($"weapon.MaximumRange (calced): {weapon.MaximumRange}");
                Helper.AppendToFile($"weapon.EffectiveRange (calced): {weapon.EffectiveRange}");
                Helper.AppendToFile($"weapon.DamagePayload.Range: {weapon.DamagePayload.Range}");
                Helper.AppendToFile($"weapon.DamagePayload.AoeRadius: {weapon.DamagePayload.AoeRadius}");
                Helper.AppendToFile($"weapon.DamagePayload.ConeRadius: {weapon.DamagePayload.ConeRadius}");
                Helper.AppendToFile($"weapon.DamagePayload.GenerateRangeValue (calced): {weapon.DamagePayload.GenerateRangeValue()}");
                Helper.AppendToFile("");
            }

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
