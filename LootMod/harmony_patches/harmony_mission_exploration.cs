using HarmonyLib;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Levels.ActorDeployment;
using PhoenixPoint.Common.Levels.MapGeneration;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Geoscape.Entities;

namespace LootMod.harmony_patches
{
    // findings:
    // private TacMissionFactionData AddEnvironmentParticipant(TacMissionData missionData, PPFactionDef faction) must be called to add the env faction for crates
    // missionData.MissionParticipants.(get the env faction from the list).ActorDeployData contains the finished crates
    // resourceCrateDeployInfo is randomly generated in AddResourceCrates() and determines the content of the crates

    [HarmonyPatch(typeof(GeoMission), "AddCratesToMissionData")]
    internal static class GeoMission_AddCratesToMissionData_Exploration
    {
        // explores how and when crates are added to missions
        public static void Prefix(GeoMission __instance, TacMissionData missionData, bool allowResourceCrates, MapPlotDef plotDef)
        {
            Helper.AppendToFile($"");
            string s = $"harmony Prefix for GeoMission.AddCratesToMissionData().";
            Helper.AppendToFile($"{s}");
            ModHandler.modInstance.Logger.LogInfo($"{s}");

            Helper.AppendToFile($"- missionData.MissionType = {missionData.MissionType.name}");
            Helper.AppendToFile($"- __instance.Site.Owner.PPFactionDef.ShortName = {__instance.Site.Owner.PPFactionDef.ShortName}");

            // MissionSpecificCrates
            Helper.AppendToFile($"");
            Helper.AppendToFile($"the following variables determine if mission specific crates should be added:");
            if (missionData.MissionType.MissionSpecificCrates == null) Helper.AppendToFile($"- missionData.MissionType.MissionSpecificCrates is null, so no mission specific crates are added");
            else
            {
                Helper.AppendToFile($"- missionData.MissionType.MissionSpecificCrates is {missionData.MissionType.MissionSpecificCrates}. mission specific crates will be added according to the following parameters:");
                ActorDeployData actorDeployData = missionData.MissionType.MissionSpecificCrates.EquipmentCratesDeployData.Clone();
                Helper.AppendToFile($"- missionData.MissionType.MissionSpecificCrates.EquipmentCratesDeployData.Clone():");
                Helper.PrintPropertiesAndFields(actorDeployData, ModHandler.modInstance, "  - ");
                Helper.AppendToFile($"- MissionSpecificCrates will contain X additional random equipment. X is a random number between {missionData.MissionType.FactionItemsRange.Min} and {missionData.MissionType.FactionItemsRange.Max}.");
            }

            // GetRandomEquipmentCrate()
            Helper.AppendToFile($"");
            Helper.AppendToFile($"the following variables determine if GetRandomEquipmentCrate() is used when there are no mission specific crates:");
            Helper.AppendToFile($"- GetRandomEquipmentCrate() will contain X random equipment (if this is <= 0 there will be no GetRandomEquipmentCrate()). X is a random number between {missionData.MissionType.FactionItemsRange.Min} and {missionData.MissionType.FactionItemsRange.Max}.");
            Helper.AppendToFile($"- Site.GeoLevel.PhoenixFaction.FactionDef.RandomCratesData?.EquipmentCratesDeployData must be not null. is it null? {(__instance.Site.GeoLevel.PhoenixFaction.FactionDef.RandomCratesData?.EquipmentCratesDeployData == null)}");
            //Helper.PrintPropertiesAndFields(__instance.Site.GeoLevel.PhoenixFaction.FactionDef.RandomCratesData?.EquipmentCratesDeployData, ModHandler.modInstance, "  - ");

            // random equipment pool
            Helper.AppendToFile($"");
            Helper.AppendToFile($"the following variables determine which if Site.Owner.Manufacture or geoLevel.GetAvailableFactionEquipment is used as pool of equipments for GetRandomEquipmentCrate and MissionSpecificCrates:");
            Helper.AppendToFile($"- Site.Owner.Manufacture is preferred and used as long as it isnt null. is it null? {(__instance.Site.Owner.Manufacture == null)}");
            if (__instance.Site.Owner.Manufacture != null)
            {
                Helper.AppendToFile($"- Site.Owner.Manufacture is used:");
                Helper.PrintPropertiesAndFields(__instance.Site.Owner.Manufacture, ModHandler.modInstance, "  - ");
                Helper.AppendToFile($"- Site.Owner.Manufacture.GetEquipment() contains these Items, though only TacticalItemDefs are used for crates:");
                foreach (ManufacturableItem equipment in __instance.Site.Owner.Manufacture.GetEquipment())
                {
                    Helper.AppendToFile($"  - {equipment.RelatedItemDef.GetType()} - {equipment.RelatedItemDef.name}");
                }
            }

            // AddResourceCrates()
            Helper.AppendToFile($"");
            Helper.AppendToFile($"the following variables determine if AddResourceCrates() is used when there are no mission specific crates and no random faction crates:");
            Helper.AppendToFile($"- bool allowResourceCrates must be true. it is {allowResourceCrates}");
            Helper.AppendToFile($"- MapPlotDef.ResourceCratesCount must be bigger than 0. it is a random int between {plotDef.ResourceCratesCount.Min} and {plotDef.ResourceCratesCount.Max}");
            Helper.AppendToFile($"- there must be more than 0 MapPlotDef.ResourceCratesDeployInfos. there are the following {plotDef.ResourceCratesDeployInfos.Length} infos:");
            foreach (var i in plotDef.ResourceCratesDeployInfos)
            {
                Helper.AppendToFile($"  - {i.SpawnWeight} - {i.CrateDef.ResourceType} - {i.CrateDef.ResourceAmount}");
                //Helper.PrintPropertiesAndFields(i.CrateDef.CrateActorDef, ModHandler.modInstance, "    - ");
            }

            Helper.AppendToFile($"");
        }
    }
}

