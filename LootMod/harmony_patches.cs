using System.Collections.Generic;
using System;
using System.Security.Policy;
using HarmonyLib;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Home.View.ViewModules;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Common.Core;
using System.Security.Cryptography;


namespace LootMod
{
    // on missing creation, add the weapons corresponsing to the GeoMission.Site.Owner Faction to the available pool of weapons for crates
    [HarmonyPatch(typeof(GeoLevelController), "GetAvailableFactionEquipment")]
    public static class GeoLevelController_GetAvailableFactionEquipment_patch
    {


        public static void Postfix(ref List<TacticalItemDef> __result)
        {
            try
            {
                ModHandler.modInstance.Logger.LogInfo($"harmony Postfix for GeoLevelController.GetAvailableFactionEquipment().");
                __result.AddRange(ModHandler.Loot.NewItems);  // TODO instead, check which weapons are in the list, and replace them with the modified versions

                // print the items currently avaialbe in crates
                int totalItems = __result.Count;
                if (totalItems > 0)
                {
                    int totalSpawnWeight = 0;
                    foreach (TacticalItemDef i in __result)
                    {
                        totalSpawnWeight += i.CrateSpawnWeight;
                    }
                    Helper.AppendToFile($"\n- {totalItems} items available for creates with {totalSpawnWeight} total CrateSpawnWeights:");
                    foreach (TacticalItemDef i in __result)
                    {
                        float spawnChance = (float)i.CrateSpawnWeight / totalSpawnWeight * 100;
                        Helper.AppendToFile($"{i.name} - {i.CrateSpawnWeight} - {spawnChance:F2}%");
                    }
                }
                else
                {
                    Helper.AppendToFile($"\n- 0 Items available for creates");
                }
                Helper.AppendToFile($"\n---\n");
            }
            catch (Exception ex)
            {
                ModHandler.modInstance.Logger.LogInfo($"Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }


    [HarmonyPatch(typeof(GeoMission), "AddCratesToMissionData")]
    internal static class GeoMission_AddCratesToMissionData
    {

        private static void Postfix(GeoMission __instance, TacMissionData missionData, bool allowResourceCrates) {
            ModHandler.modInstance.Logger.LogInfo($"harmony Postfix for GeoMission.AddCratesToMissionData().");
            ModHandler.modInstance.Logger.LogInfo($"- bool allowResourceCrates = {allowResourceCrates}");
            ModHandler.modInstance.Logger.LogInfo($"- (Site.Owner.Manufacture == null) is {(__instance.Site.Owner.Manufacture == null)}");
            ModHandler.modInstance.Logger.LogInfo($"- Faction = {__instance.Site.Owner.PPFactionDef.ShortName}");

            //Helper.AppendToFile($"\n- GeoMission __instance");
            //Helper.PrintPropertiesAndFields(__instance, ModHandler.ModInstance); 

            //Helper.AppendToFile($"\n- GeoMission __instance.Site");
            //Helper.PrintPropertiesAndFields(__instance.Site, ModHandler.ModInstance); 

            //Helper.AppendToFile($"\n- GeoMission __instance.Site.Owner");
            //Helper.PrintPropertiesAndFields(__instance.Site.Owner, ModHandler.modInstance); 
            //Helper.AppendToFile($"\n- GeoMission __instance.Site.Owner.PPFactionDef");
            //Helper.PrintPropertiesAndFields(__instance.Site.Owner.PPFactionDef, ModHandler.modInstance);

            if (__instance.Site.Owner.Manufacture != null)
            {
            Helper.AppendToFile($"\n- GeoMission __instance.Site.Owner.Manufacture");
            Helper.PrintPropertiesAndFields(__instance.Site.Owner.Manufacture, ModHandler.modInstance); 
            }

            //Helper.AppendToFile($"\n- GeoMission __instance.Site.GeoLevel.Factions");
            //foreach (GeoFaction f in __instance.Site.GeoLevel.Factions)
            //{
            //    Helper.AppendToFile(f.PPFactionDef.ShortName);

            //}

            //Helper.AppendToFile($"\n- GeoMission __instance.MissionDef");
            //Helper.PrintPropertiesAndFields(__instance.MissionDef, ModHandler.ModInstance); 

            //Helper.AppendToFile($"\n- AddCratesToMissionData arg TacMissionData missionData");
            //Helper.PrintPropertiesAndFields(missionData, ModHandler.ModInstance);

            //Helper.AppendToFile($"\n- AddCratesToMissionData arg TacMissionData missionData.MissionType");
            //Helper.PrintPropertiesAndFields(missionData.MissionType, ModHandler.ModInstance); 
        }

    }



  
}
