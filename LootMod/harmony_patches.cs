using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Geoscape.Levels;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Equipments;

namespace LootMod
{

    // animation contain a list of EquipmentDefs for which they are valid. my modified items are not in that list. so no animations are found for them.
    // TacActorAnimActionEquipmentFilteredDef.EquipmentMatch is used to find the equipments valid for an animation.
    // The first time the EquipmentMatch is called, this prefix will add my modfied items are added to the Equipments list of each AnimAction where its vanilla counterpart is present
    // I can identify the first time EquipmentMatch is called by checking if the internal cache (private HashSet<int> _equipmentIds) is empty
    // unfotunately, TacActorAnimActionEquipmentFilteredDef.EquipmentMatch check two different lists of EquipmentDefs, so I check which one it uses and modify that one.
    // TODO find out if both are used in practice, and if I can remove the code related ot the unused one
    [HarmonyPatch(typeof(TacActorAnimActionEquipmentFilteredDef), "EquipmentMatch")]
    public static class TacActorAnimActionEquipmentFilteredDef_EquipmentMatch_Patch
    {
        public static void Prefix(TacActorAnimActionEquipmentFilteredDef __instance, Equipment equipment, HashSet<int> ____equipmentIds)
        {
            try
            {
                string s = $"harmony Prefix for TacActorAnimActionEquipmentFilteredDef.EquipmentMatch(). __instance.name = {__instance.name}, searching for {equipment.EquipmentDef.name}, ID {equipment.EquipmentDef.GetInstanceID()}";
                ModHandler.modInstance.Logger.LogInfo(s);
                Helper.AppendToFile($"{s}");

                // the EquipmentMatch() will immediately return false if equipment == null, so no need for me to edit anything
                if (equipment == null)
                {
                    Helper.AppendToFile($"equipment was null, no changes.");
                    return;
                }

                EquipmentDef[] vanillaEquipmentsArray;
                HashSet<int> equipmentIdsToCheck = null;
                // check which list of EquipmentDefs will be used, assign the corresponding EquipmentDef[] to EquipmentsToEdit and the HashSet<int> to equipmentIdsToCheck
                // EquipmentDef[] __instance.EquipmentList.Equipments seems to be prefered over EquipmentDef[] __instance.Equipments
                if (__instance.EquipmentList == null)
                {
                    Helper.AppendToFile($"__instance.Equipments is used.");
                    vanillaEquipmentsArray = __instance.Equipments;
                    equipmentIdsToCheck = ____equipmentIds;
                }
                else
                {
                    Helper.AppendToFile($"__instance.EquipmentList.Equipments is used.");
                    vanillaEquipmentsArray = __instance.EquipmentList.Equipments;

                    // Use reflection to access the private field
                    FieldInfo fieldInfo = __instance.EquipmentList.GetType().GetField("_equipmentIds", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo != null)
                    {
                        equipmentIdsToCheck = (HashSet<int>)fieldInfo.GetValue(__instance.EquipmentList);
                    }
                    else
                    {
                        Helper.AppendToFile($"Failed to access the private field '_equipmentIds'.");
                    }
                }


                if (vanillaEquipmentsArray == null)
                {
                    Helper.AppendToFile($"EquipmentsArrayToReplace is null. this should not happen!");
                    return;
                }
                Helper.AppendToFile($"there are {vanillaEquipmentsArray.Length} equipments for this animation currently.");

                // check if the EquipmentMatch is called for the first time
                if (equipmentIdsToCheck != null)
                {
                    Helper.AppendToFile($"equipmentIdsToCheck is not null, items should have already been added.");
                    return;
                }

                // this is the first time EquipmentMatch is called -> add the modified items to the Equipments list
                Helper.AppendToFile($"equipmentIdsToCheck is null -> adding modified items now.");

                // Create a new list of equipments and add the modified items
                List<EquipmentDef> newEquipmentList = new List<EquipmentDef>(vanillaEquipmentsArray);
                foreach (EquipmentDef equipmentFromOriginalArray in vanillaEquipmentsArray)
                {
                    // find the modified versions, if there are any for this equipment
                    if (ModHandler.Loot.NewItems.TryGetValue(equipmentFromOriginalArray.name, out List<TacticalItemDef> modifiedItems))
                    {
                        // for some items all modification validation checks failed, so modifiedItems is empty
                        if (modifiedItems.Count == 0)
                        {
                            Helper.AppendToFile($"No modified versions of {equipmentFromOriginalArray.name} were found.");
                            continue;
                        }

                        // if it contains the first of the modified items, it must contain all of them
                        if (newEquipmentList.Contains(modifiedItems[0]))
                        {
                            // TODO check if this is necessary. equipmentIdsToCheck above should make this if clause unnecessary.
                            Helper.AppendToFile($"Modified versions of {equipmentFromOriginalArray.name} were already added previously.");
                        }
                        else
                        {
                            // note for the cast: EquipmentDef is a TacticalItemDef -> not all TacticalItemDefs are EquipmentDefs. there is no chance
                            // to add a non-EquipmentDef to the Equipments list though, since the modified versions are of the same type as the original version,
                            // and the original version must be an equipment if it is found in the Equipments array)
                            newEquipmentList.AddRange(modifiedItems.Cast<EquipmentDef>());
                            Helper.AppendToFile($"Added {modifiedItems.Count} modified versions of {equipmentFromOriginalArray.name} - .GetInstanceID() = {equipmentFromOriginalArray.GetInstanceID()}");
                        }
                    }
                }

                // replace the vanilla EquipmentsDef[] with the new one
                if (__instance.EquipmentList == null)
                {
                    Helper.AppendToFile($"replacing __instance.Equipments.");
                    __instance.Equipments = newEquipmentList.ToArray();
                }
                else
                {
                    Helper.AppendToFile($"replacing __instance.EquipmentList.Equipments.");
                    __instance.EquipmentList.Equipments = newEquipmentList.ToArray();
                }
            }
            catch (Exception ex)
            {
                ModHandler.modInstance.Logger.LogInfo($"Exception in Postfix: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public static void Postfix(TacActorAnimActionEquipmentFilteredDef __instance, Equipment equipment, bool __result, HashSet<int> ____equipmentIds)
        {
            try
            {
                string s = $"harmony Postfix for TacActorAnimActionEquipmentFilteredDef.EquipmentMatch(). __instance.name = {__instance.name}, searching for {equipment.EquipmentDef.name}, ID {equipment.EquipmentDef.GetInstanceID()}";
                ModHandler.modInstance.Logger.LogInfo(s);
                Helper.AppendToFile($"{s}");

                HashSet<int> equipmentIdsToCheck = null;
                if (__instance.Equipments == null) Helper.AppendToFile($"__instance.Equipments is null.");
                else
                {
                    Helper.AppendToFile($"__instance.Equipments is not null.");
                    if (____equipmentIds == null) Helper.AppendToFile($"Private hash for __instance.Equipments is null.");
                    else
                    {
                        Helper.AppendToFile($"Private hash for __instance.Equipments is not null.");
                        equipmentIdsToCheck = ____equipmentIds;
                    }
                }

                if (__instance.EquipmentList == null) Helper.AppendToFile($"__instance.EquipmentList is null.");
                else
                {
                    Helper.AppendToFile($"__instance.EquipmentList is not null.");
                    FieldInfo fieldInfoEquipmentList = __instance.EquipmentList.GetType().GetField("_equipmentIds", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfoEquipmentList != null)
                    {
                        var equipmentListIds = (HashSet<int>)fieldInfoEquipmentList.GetValue(__instance.EquipmentList);
                        if (equipmentListIds == null)
                        {
                            Helper.AppendToFile($"Private hash for __instance.EquipmentList.Equipments is null.");
                        }
                        else
                        {
                            Helper.AppendToFile($"Private hash for __instance.EquipmentList.Equipments is not null.");
                            equipmentIdsToCheck = equipmentListIds;
                        }
                    }
                }

                if (equipmentIdsToCheck != null)
                {
                    Helper.AppendToFile($"equipmentIdsToCheck is not null.");
                    Helper.AppendToFile($"equipmentIdsToCheck.Count = {equipmentIdsToCheck.Count}");
                    Helper.AppendToFile($"equipmentIdsToCheck.Contains({equipment.EquipmentDef.GetInstanceID()}) = {equipmentIdsToCheck.Contains(equipment.EquipmentDef.GetInstanceID())}");
                }
                else
                {
                    Helper.AppendToFile($"equipmentIdsToCheck is null.");
                }
            }
            catch (Exception ex)
            {
                ModHandler.modInstance.Logger.LogInfo($"Exception in Postfix: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }

    // on mission creation, add the relevant modified to the available pool of weapons for crates
    // TODO choose those depending on the GeoMission.Site.Owner Faction 
    // TODO or replace the items originally available with their respective modified versions
    [HarmonyPatch(typeof(GeoLevelController), "GetAvailableFactionEquipment")]
    public static class GeoLevelController_GetAvailableFactionEquipment_patch
    {
        public static void Postfix(ref List<TacticalItemDef> __result)
        {
            try
            {
                ModHandler.modInstance.Logger.LogInfo($"harmony Postfix for GeoLevelController.GetAvailableFactionEquipment().");
                var itemsToRemove = new List<TacticalItemDef>(); // Collect items to remove
                var itemsToAdd = new List<TacticalItemDef>();    // Collect items to add
                foreach (TacticalItemDef item in __result)
                {
                    // find the modified replacements, if there are any for this item
                    if (ModHandler.Loot.NewItems.TryGetValue(item.name, out List<TacticalItemDef> replacementNewItems))
                    {
                        itemsToRemove.Add(item); // Mark the original item for removal
                        itemsToAdd.AddRange(replacementNewItems); // Collect new items to add
                    }
                }
                foreach (var item in itemsToRemove)
                {
                    __result.Remove(item);  // Remove the original items
                }
                __result.AddRange(itemsToAdd);  // Add the replacement items

                // print the items currently availabe in crates for debugging
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
                ModHandler.modInstance.Logger.LogInfo($"Printed all {totalItems} items available for crates to file.");
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
        private static void Postfix(GeoMission __instance, TacMissionData missionData, bool allowResourceCrates)
        {
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
