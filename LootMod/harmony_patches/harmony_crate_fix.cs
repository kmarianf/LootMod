using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Base;
using HarmonyLib;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Levels.ActorDeployment;
using PhoenixPoint.Common.Levels.MapGeneration;
using PhoenixPoint.Common.Levels.Missions;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Tactical.Entities.ActorsInstance;
using PhoenixPoint.Tactical.Entities.Equipments;
using static PhoenixPoint.Common.Entities.Items.InventorySupplementComponentDef;

namespace LootMod.harmony_patches
{
    // there are two pools used to randomly draw item from for crates:
    // PhoenixPoint.Geoscape.Levels.GeoLevelController.GetAvailableFactionEquipment()
    // PhoenixPoint.Common.Entities.Items.ItemManufacturing.GetEquipment()
    // I just catch the crated after they are added to the mission and replace the original items with my modified versions (so I ignore the pools)
    [HarmonyPatch(typeof(GeoMission), "AddCratesToMissionData")]
    internal static class GeoMission_AddCratesToMissionData
    {
        /// <summary>this a) add a few small crates to missions that dont have any and b) replaces the content of the crates with modded items</summary>
        public static void Postfix(GeoMission __instance, TacMissionData missionData, bool allowResourceCrates, MapPlotDef plotDef)
        {
            Helper.AppendToFile($"\nharmony Postfix for GeoMission.AddCratesToMissionData().");

            // only add additional crates if the mission doesnt automatically pick up all items from the ground on completion
            if (!missionData.MissionType.DontRecoverItems)
            {
                Helper.AppendToFile($"missionData.MissionType.DontRecoverItems is {missionData.MissionType.DontRecoverItems}, so we do not provide additional crates.");
                return;
            }

            // the env faction has the crates.
            PPFactionDef environmentFactionDef = __instance.GameController.GetComponent<SharedData>().EnvironmentFactionDef;
            // find the env faction within missionData.MissionParticipants with AddEnvironmentParticipant()
            // AddEnvironmentParticipant finds or creates the env faction and returns it. its private, so we need the reflection workaround
            MethodInfo AddEnvironmentParticipantMethod = typeof(GeoMission).GetMethod("AddEnvironmentParticipant", BindingFlags.NonPublic | BindingFlags.Instance);
            TacMissionFactionData tacMissionEnvFactionData = (TacMissionFactionData)AddEnvironmentParticipantMethod.Invoke(__instance, new object[] { missionData, environmentFactionDef });

            // any resource crates added during AddCratesToMissionData() are somewhere in here. I will add extra crates here if there are none yet.
            Helper.AppendToFile($"- {tacMissionEnvFactionData.ActorDeployData.Count} tacMissionEnvFactionData.ActorDeployData:");
            foreach (var actor in tacMissionEnvFactionData.ActorDeployData)
            {
                Helper.PrintPropertiesAndFields(actor, ModHandler.modInstance, "  - ");
                if (actor.InstanceDef != null)
                {
                    Helper.AppendToFile($"- actor.InstanceDef:");
                    Helper.PrintPropertiesAndFields(actor.InstanceDef, ModHandler.modInstance, "  - ");
                }
            }

            //if there are no crates, add a few minor crates. this is pretty much a straight copy from the original GetRandomEquipmentCrate()
            if (tacMissionEnvFactionData.ActorDeployData.Count != 0) Helper.AppendToFile($"there are already {tacMissionEnvFactionData.ActorDeployData.Count} actors in tacMissionEnvFactionData.ActorDeployData, so no extra crates.");
            else if (!LootMod.Loot.ALLOW_ADDITIONAL_CRATES) return;
            else
            {
                Helper.AppendToFile($"starting to add extra crates similar to GetRandomEquipmentCrate()");

                // number of creates
                Helper.AppendToFile($"- env faction InitialDeploymentPoints are {tacMissionEnvFactionData.InitialDeploymentPoints}");
                if (tacMissionEnvFactionData.InitialDeploymentPoints < 10)
                {
                    // we need 10 per resource crate, so increase the InitialDeploymentPoints
                    tacMissionEnvFactionData.InitialDeploymentPoints = 10;
                    Random random = new Random();
                    while (random.Next(2) == 1) // 50% chance to add another 10 points, repeating until the chance is not met
                    {
                        tacMissionEnvFactionData.InitialDeploymentPoints += 10;
                    }
                    Helper.AppendToFile($"- env faction InitialDeploymentPoints was set to {tacMissionEnvFactionData.InitialDeploymentPoints}");
                }

                ActorDeployData randomCratesActorDeployData = __instance.Site.GeoLevel.PhoenixFaction.FactionDef.RandomCratesData.EquipmentCratesDeployData.Clone();
                Helper.AppendToFile($"- randomCratesActorDeployData.DeployCost = {randomCratesActorDeployData.DeployCost}");
                List<TacticalItemDef> list = null;
                if (__instance.Site.Owner.Manufacture == null)
                {
                    Helper.AppendToFile($"- Site.Owner.Manufacture is null, so we will use GetAvailableFactionEquipment() as equipment pool");
                    list = __instance.Site.GeoLevel.GetAvailableFactionEquipment(onlyDiscoveredFactions: true);
                }
                else
                {
                    Helper.AppendToFile($"- Site.Owner.Manufacture is not null, so we will use it as equipment pool");
                    list = (from i in __instance.Site.Owner.Manufacture.GetEquipment() select (TacticalItemDef)i.RelatedItemDef).ToList();
                }
                Helper.AppendToFile($"- List<TacticalItemDef> contains {list.Count} items.");
                if (list.Count == 0)
                {
                    return;
                }
                TacEquipmentCrateData tacEquipmentCrateData = (TacEquipmentCrateData)(randomCratesActorDeployData.ActorInstance = new TacEquipmentCrateData
                {
                    ComponentSetTemplate = randomCratesActorDeployData.InstanceDef.InstanceData.ComponentSetTemplate,
                    Quantity = new Base.Utils.RangeDataInt(1, 3),
                    Items = new ItemChancePair[list.Count()]
                });
                list.Shuffle();
                for (int j = 0; j < list.Count(); j++)
                {
                    ItemDef itemDef = list[j];
                    tacEquipmentCrateData.Items[j] = new ItemChancePair
                    {
                        ChanceToPresent = itemDef.CrateSpawnWeight,
                        ItemDef = itemDef
                    };
                }
                randomCratesActorDeployData.InstanceDef = null;
                tacMissionEnvFactionData.ActorDeployData.Add(randomCratesActorDeployData);
            }
            Helper.AppendToFile($"");


            // now there are definitely crates. but they still contain the original items. so I replace them with the modded items.
            Helper.AppendToFile($"replacing the content of the crates with modded items");
            Helper.AppendToFile($"- tacMissionEnvFactionData.ActorDeployData contains {tacMissionEnvFactionData.ActorDeployData.Count} actors. will try to find TacEquipmentCrateData...");
            foreach (var actor in tacMissionEnvFactionData.ActorDeployData)
            {
                if (actor.ActorInstance is TacEquipmentCrateData crateData)
                {
                    Helper.AppendToFile($"- found a TacEquipmentCrateData actor! replacing its items now.");

                    Helper.AppendToFile($"- replacing {crateData.Items.Length} Items:");
                    ItemChancePair[] itemChancePairs = crateData.Items;
                    crateData.Items = replaceItemsWithModifiedVersions(itemChancePairs.ToList()).ToArray();
                    Helper.AppendToFile($"- replacing {crateData.AdditionalItems.Count} AdditionalItems:");
                    List<ItemChancePair> additionalItemChancePairs = crateData.AdditionalItems;
                    crateData.AdditionalItems = replaceItemsWithModifiedVersions(additionalItemChancePairs);
                }
            }
        }

        private static List<ItemChancePair> replaceItemsWithModifiedVersions(List<ItemChancePair> originalItemChancePairs)
        {
            Helper.AppendToFile($"replaced?,Name,type,# modified versions,Orig spawn weight,New total spawn weight");

            List<ItemChancePair> newItemChancePairs = new List<ItemChancePair>();
            foreach (ItemChancePair originalItemChancePair in originalItemChancePairs)
            {
                // find the modified replacements, if there are any for this item
                if (ModHandler.Loot.NewItems.TryGetValue(originalItemChancePair.ItemDef.name, out List<TacticalItemDef> replacementItems) && replacementItems.Count > 0)
                {
                    // found modified versions: create new ItemChancePairs that will replace the original one
                    int totalSpawnWeight = 0;
                    foreach (TacticalItemDef replacementItem in replacementItems)
                    {
                        newItemChancePairs.Add(new ItemChancePair
                        {
                            ChanceToPresent = replacementItem.CrateSpawnWeight,
                            ItemDef = replacementItem
                        });
                        totalSpawnWeight += replacementItem.CrateSpawnWeight;
                    }
                    Helper.AppendToFile($"yes,{originalItemChancePair.ItemDef.name},{originalItemChancePair.ItemDef.GetType()},{replacementItems.Count},{originalItemChancePair.ItemDef.CrateSpawnWeight},{totalSpawnWeight}");
                }
                else
                {
                    // this item has no modified versions: keep the original item
                    newItemChancePairs.Add(originalItemChancePair);
                    Helper.AppendToFile($"no,{originalItemChancePair.ItemDef.name},{originalItemChancePair.ItemDef.GetType()},0,{originalItemChancePair.ChanceToPresent},{originalItemChancePair.ChanceToPresent}");
                }
            }
            return newItemChancePairs;
        }
    }
}
