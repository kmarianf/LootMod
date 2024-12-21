using System;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Weapons;
using UnityEngine;

namespace LootMod
{
    //[HarmonyPatch(typeof(TacticalPerception), nameof(TacticalPerception.GetShotOrigin))]
    public static class TacticalPerception_GetShotOrigin_patch
    {
        public static void Prefix(TacticalPerception __instance, Weapon weapon, Vector3 position, TacticalAbilityTarget target, string projectileOrigin = null, int shootPointIndex = 0)
        {
            try
            {
                ModHandler.modInstance.Logger.LogInfo($"harmony Prefix for TacticalPerception.GetShotOrigin().");
                Helper.AppendToFile($"");
                Helper.AppendToFile($"----------------------------------------------------------------------------------------");
                Helper.AppendToFile($"");

                // Create the context and log its properties
                TacActorShootAnimActionDef.ShootContext shootContext = TacActorShootAnimActionDef.MakeContext(__instance.TacActor, weapon, target, position);
                Helper.AppendToFile($"ShootContext created: {shootContext}");
                Helper.PrintPropertiesAndFields(shootContext, ModHandler.modInstance);

                //Helper.AppendToFile($"");
                //Helper.AppendToFile($"checking __instance.TacActor");
                //Helper.PrintPropertiesAndFields(__instance.TacActor, ModHandler.modInstance);

                //Helper.AppendToFile($"");
                //Helper.AppendToFile($"checking __instance.TacActor.ActorAnimActions");
                //Helper.PrintPropertiesAndFields(__instance.TacActor.ActorAnimActions, ModHandler.modInstance);

                Helper.AppendToFile($"");
                Helper.AppendToFile($"checking TacActorShootAnimActionDef");
                var animActionsDef = __instance.TacActor.ActorAnimActions.TacActorAnimActionsDef;
                if (animActionsDef == null)
                {
                    Helper.AppendToFile($"TacActorAnimActionsDef is null");
                }
                else
                {
                    Helper.AppendToFile($"TacActorAnimActionsDef is not null");
                    Helper.PrintPropertiesAndFields(__instance.TacActor.ActorAnimActions.TacActorAnimActionsDef, ModHandler.modInstance);
                    foreach (var animAction in animActionsDef.AnimActions)
                    {
                        Helper.AppendToFile($"");
                        Helper.PrintPropertiesAndFields(animAction, ModHandler.modInstance);
                    }
                }

                Helper.AppendToFile($"");
                Helper.AppendToFile($"checking GetAnimAction");
                var action = __instance.TacActor.ActorAnimActions.GetAnimAction<TacActorShootAnimActionDef>(shootContext);
                if (action == null)
                {
                    Helper.AppendToFile($"action is null");
                }
                else
                {
                    Helper.AppendToFile($"");
                    Helper.AppendToFile($"checking AnimationClip shootPose");
                    AnimationClip shootPose = action.ShootPose;
                    if (shootPose == null)
                    {
                        Helper.AppendToFile($"shootPose is null");
                    }
                    else
                    {
                        Helper.AppendToFile($"shootPose is not null");
                        Helper.PrintPropertiesAndFields(shootPose, ModHandler.modInstance);
                    }
                }
            }
            catch (Exception ex)
            {
                ModHandler.modInstance.Logger.LogInfo($"Exception in Prefix: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
