using Base.Entities.Abilities;
using Base.Entities.Statuses;
using Base.UI;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Statuses;

namespace LootMod.Modifications.Abilities
{
    internal class NewAbilities
    {
        public static ApplyStatusAbilityDef Regeneration_Legs_Passive_AbilityDef;
        public static ApplyStatusAbilityDef Regeneration_Head_Passive_AbilityDef;

        static NewAbilities()
        {
            // create the new abilities

            ApplyStatusAbilityDef regenerationAbility = DefCache.GetDef<ApplyStatusAbilityDef>("Regeneration_Torso_Passive_AbilityDef");
            // original description (KEY_REGENERATION_TORSO_DESCRIPTION) = Restore 10 Hit Points to arms, torso and General Hit Points each turn

            // legs regeneration
            Regeneration_Legs_Passive_AbilityDef = (ApplyStatusAbilityDef)_createBaseCopy(regenerationAbility, "Regeneration_Legs_Passive");
            Regeneration_Legs_Passive_AbilityDef.name = "Regeneration_Legs_Passive_AbilityDef";
            ((HealthChangeStatusDef)Regeneration_Legs_Passive_AbilityDef.StatusDef).BodypartSlotNames = new string[] { "LeftLeg", "RightLeg" };
            string key = "LOC_DESC_LOOT_Regeneration_Legs_Passive_AbilityDef";
            string desc = "Restore 10 Hit Points to legs and General Hit Points each turn";
            Regeneration_Legs_Passive_AbilityDef.ViewElementDef.Description = new LocalizedTextBind(key);
            ((HealthChangeStatusDef)Regeneration_Legs_Passive_AbilityDef.StatusDef).Visuals.Description = new LocalizedTextBind(key);
            ModHandler.LocalizationHandler.AddLine(key, desc);

            //Regeneration_Legs_Passive_AbilityDef.ViewElementDef.Description
            // head regeneration
            Regeneration_Head_Passive_AbilityDef = (ApplyStatusAbilityDef)_createBaseCopy(regenerationAbility, "Regeneration_Head_Passive");
            Regeneration_Head_Passive_AbilityDef.name = "Regeneration_Head_Passive_AbilityDef";
            ((HealthChangeStatusDef)Regeneration_Head_Passive_AbilityDef.StatusDef).BodypartSlotNames = new string[] { "Head" };
            key = "LOC_DESC_LOOT_Regeneration_Head_Passive_AbilityDef";
            desc = "Restore 10 Hit Points to head and General Hit Points each turn";
            Regeneration_Head_Passive_AbilityDef.ViewElementDef.Description = new LocalizedTextBind(key);
            ((HealthChangeStatusDef)Regeneration_Head_Passive_AbilityDef.StatusDef).Visuals.Description = new LocalizedTextBind(key);
            ModHandler.LocalizationHandler.AddLine(key, desc);
        }

        private static AbilityDef _createBaseCopy(AbilityDef originalAbility, string newID)
        {
            string newName = $"{newID}_AbilityDef";
            AbilityDef newAbility = (AbilityDef)DefCache.Repo.CreateDef($"LOOT_ID_{newName}", originalAbility);
            if (newAbility is TacticalAbilityDef newTacticalAbility)
            {
                TacticalAbilityViewElementDef newViewElementDef = (TacticalAbilityViewElementDef)DefCache.Repo.CreateDef($"LOOT_ID_VED_{newName}", newTacticalAbility.ViewElementDef);
                newViewElementDef.name = $"E_ViewElement [{newName}]";
                //newViewElementDef.Name = $"LOOT_VED_NAME_{newName}";  // its just "Regeneration" for the current cases
                newTacticalAbility.ViewElementDef = newViewElementDef;
            }
            if (newAbility is ApplyStatusAbilityDef newStatusAbility)
            {
                StatusDef newStatusDef = (StatusDef)DefCache.Repo.CreateDef($"LOOT_{newID}_StatusDef", newStatusAbility.StatusDef);
                string statusName = $"LOOT_{newID}_Constant_StatusDef";
                newStatusDef.name = statusName;
                newStatusAbility.StatusDef = newStatusDef;
                if (newStatusDef is HealthChangeStatusDef newhealthChangeStatusDef)
                {
                    // the HealthChangeStatusDefs also have ViewElementDefs
                    ViewElementDef newViewElementDef = (ViewElementDef)DefCache.Repo.CreateDef($"LOOT_ID_VED_STATUS_{newName}", newhealthChangeStatusDef.Visuals);
                    newViewElementDef.name = $"E_Visuals [{statusName}]";
                    //newViewElementDef.Name = $"LOOT_VED_NAME_{newName}";  // its just "Regeneration" for the current cases
                    newhealthChangeStatusDef.Visuals = newViewElementDef;
                }
            }
            return newAbility;
        }
    }
}
