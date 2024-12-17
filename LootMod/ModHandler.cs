using System;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
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
                Loot = new Loot(modInstance, defCache);
                Loot.InitModifiedItems();
                defCache = new DefCache();
                //Exploration();
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

            WeaponDef origAres = defCache.GetDef<WeaponDef>("PX_AssaultRifle_WeaponDef");
            Helper.PrintPropertiesAndFields(origAres, modInstance);
            Helper.AppendToFile("");
            Helper.PrintPropertiesAndFields(origAres.DamagePayload, modInstance);
            Helper.AppendToFile("");
            foreach (DamageKeywordPair d in origAres.DamagePayload.DamageKeywords)
            {
                Helper.PrintPropertiesAndFields(d, modInstance);
                Helper.AppendToFile("");
                Helper.PrintPropertiesAndFields(d.DamageKeywordDef, modInstance);
                Helper.AppendToFile("");
            }
            modInstance.Logger.LogInfo($"");




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


            //foreach (InventoryComponentDef def in defCache.Repo.GetAllDefs<InventoryComponentDef>())
            //{
            //    int totalItems = def.ItemDefs.Count();
            //    if (totalItems > 0)
            //    {
            //        Helper.AppendToFile($"- {def.name}");
            //        Helper.PrintPropertiesAndFields(def, modInstance);
            //        Helper.AppendToFile($"- ItemDefs of {def.name} + CrateSpawnWeights:");
            //        int totalSpawnWeight = 0;
            //        foreach (ItemDef ItemDef in def.ItemDefs)
            //        {
            //            Helper.AppendToFile($"{ItemDef.name} - {ItemDef.CrateSpawnWeight}");
            //            //Helper.PrintPropertiesAndFields(ItemDef, ModInstance);
            //            totalSpawnWeight += ItemDef.CrateSpawnWeight;
            //        }
            //        Helper.AppendToFile($"=> {totalItems} ItemDefs with {totalSpawnWeight} total CrateSpawnWeights");
            //    }
            //    else
            //    {
            //        Helper.AppendToFile($"- {def.name} has 0 ItemDefs");
            //    }
            //    Helper.AppendToFile($"\n---\n");
            //}
        }
    }
}
