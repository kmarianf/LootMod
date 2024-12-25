using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Equipments;

namespace LootMod.harmony_patches
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
                //string s = $"harmony Prefix for TacActorAnimActionEquipmentFilteredDef.EquipmentMatch(). __instance.name = {__instance.name}, searching for {equipment.EquipmentDef.name}, ID {equipment.EquipmentDef.GetInstanceID()}";
                //ModHandler.modInstance.Logger.LogInfo(s);
                //Helper.AppendToFile($"{s}");

                // the EquipmentMatch() will immediately return false if equipment == null, so no need for me to edit anything
                // I havent noticed that this actually happens
                if (equipment == null)
                {
                    //Helper.AppendToFile($"equipment was null, no changes.");
                    return;
                }

                EquipmentDef[] vanillaEquipmentsArray;
                HashSet<int> equipmentIdsToCheck = null;
                // check which list of EquipmentDefs will be used, assign the corresponding EquipmentDef[] to EquipmentsToEdit and the HashSet<int> to equipmentIdsToCheck
                // EquipmentDef[] __instance.EquipmentList.Equipments seems to be prefered over EquipmentDef[] __instance.Equipments
                if (__instance.EquipmentList == null)
                {
                    //Helper.AppendToFile($"__instance.Equipments is used.");
                    vanillaEquipmentsArray = __instance.Equipments;
                    equipmentIdsToCheck = ____equipmentIds;
                }
                else
                {
                    //Helper.AppendToFile($"__instance.EquipmentList.Equipments is used.");
                    vanillaEquipmentsArray = __instance.EquipmentList.Equipments;

                    // Use reflection to access the private field, as harmony doesnt provide a convenient way
                    FieldInfo fieldInfo = __instance.EquipmentList.GetType().GetField("_equipmentIds", BindingFlags.NonPublic | BindingFlags.Instance);
                    equipmentIdsToCheck = (HashSet<int>)fieldInfo.GetValue(__instance.EquipmentList);
                }

                if (vanillaEquipmentsArray == null)
                {
                    // doesnt seem to happen. good.
                    Helper.AppendToFile($"EquipmentsArrayToReplace is null. this should not happen!");
                    return;
                }
                //Helper.AppendToFile($"there are {vanillaEquipmentsArray.Length} equipments for this animation currently.");

                // check if the EquipmentMatch is called for the first time (tested and works, though new TacActorAnimActionEquipmentFilteredDef are created frequently)
                // TODO find another way to better cache the new equipment lists for each animation id re-creating them every time takes too long
                if (equipmentIdsToCheck != null)
                {
                    //Helper.AppendToFile($"equipmentIdsToCheck is not null, items should have already been added.");
                    return;
                }

                // this is the first time EquipmentMatch is called -> add the modified items to the Equipments list
                //Helper.AppendToFile($"equipmentIdsToCheck is null -> adding modified items now.");

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
                            //Helper.AppendToFile($"No modified versions of {equipmentFromOriginalArray.name} were found.");
                            continue;
                        }

                        // note for the cast: EquipmentDef is a TacticalItemDef -> not all TacticalItemDefs are EquipmentDefs. there is no chance
                        // to add a non-EquipmentDef to the Equipments list though, since the modified versions are of the same type as the original version,
                        // and the original version must be an equipment if it is found in the Equipments array)
                        newEquipmentList.AddRange(modifiedItems.Cast<EquipmentDef>());
                        //Helper.AppendToFile($"Added {modifiedItems.Count} modified versions of {equipmentFromOriginalArray.name} - .GetInstanceID() = {equipmentFromOriginalArray.GetInstanceID()}");
                    }
                }

                // replace the vanilla EquipmentsDef[] with the new one
                if (__instance.EquipmentList == null)
                {
                    //Helper.AppendToFile($"replacing __instance.Equipments.");
                    __instance.Equipments = newEquipmentList.ToArray();
                }
                else
                {
                    //Helper.AppendToFile($"replacing __instance.EquipmentList.Equipments.");
                    __instance.EquipmentList.Equipments = newEquipmentList.ToArray();
                }
            }
            catch (Exception ex)
            {
                ModHandler.modInstance.Logger.LogInfo($"Exception in Postfix: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
