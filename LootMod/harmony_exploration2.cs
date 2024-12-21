using System;
using System.Reflection;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Equipments;
namespace LootMod
{

    // Patch to add logging to the TacActorShootAnimActionDef.Match method
    [HarmonyPatch(typeof(TacActorShootAnimActionDef), "Match")]
    public static class TacActorShootAnimActionDef_Match_Patch
    {
        private static MethodInfo bodypartsMatchMethod = typeof(TacActorAnimActionEquipmentFilteredDef).GetMethod("BodypartsMatch", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo hasEquipmentFilterMethod = typeof(TacActorAnimActionEquipmentFilteredDef).GetMethod("HasEquipmentFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo equipmentMatchMethod = typeof(TacActorAnimActionEquipmentFilteredDef).GetMethod("EquipmentMatch", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Prefix(TacActorShootAnimActionDef __instance, object context, ref bool __state)
        {
            try
            {
                ModHandler.modInstance.Logger.LogInfo($"harmony Prefix for TacActorShootAnimActionDef.Match(). __instance.name = {__instance.name}");
                Helper.AppendToFile($"\n\nharmony Prefix for TacActorShootAnimActionDef.Match(). __instance.name = {__instance.name}");

                if (context is TacActorShootAnimActionDef.ShootContext shootContext)
                {
                    Equipment weapon = shootContext.Weapon;
                    Helper.AppendToFile($"ShootContext: TacticalActor = {shootContext.TacticalActor}, Weapon = {weapon?.DisplayName}, GetInstanceID() = {weapon.EquipmentDef.GetInstanceID()}");

                    bool bodypartsMatch = (bool)bodypartsMatchMethod.Invoke(__instance, new object[] { shootContext.TacticalActor });
                    bool hasEquipmentFilter = (bool)hasEquipmentFilterMethod.Invoke(__instance, null);
                    bool equipmentMatch = (bool)equipmentMatchMethod.Invoke(__instance, new object[] { weapon });

                    Helper.AppendToFile($"BodypartsMatch: {bodypartsMatch}, HasEquipmentFilter: {hasEquipmentFilter}, EquipmentMatch: {equipmentMatch}");
                }
                else
                {
                    Helper.AppendToFile($"Context is not a ShootContext. Context type: {context.GetType().Name}");
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
