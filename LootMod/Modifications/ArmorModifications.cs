using System;
using System.Collections.Generic;
using System.Linq;
using LootMod.Modifications.Abilities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace LootMod.Modifications
{
    public class NegativeArmorModification : NegativeModification
    {
        public override string Name => "Soft";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveArmorModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.Armor;
            float newValue = (float)Math.Floor(item.Armor * 0.75);
            Diff = origValue - newValue;
            item.Armor = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff:F0} Armor";
    }

    public class PositiveArmorModification : PositiveModification
    {
        public override string Name => "Armored";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.Armor;
            float newValue = (float)Math.Ceiling(item.Armor * 1.25);
            Diff = newValue - origValue;
            item.Armor = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff:F0} Armor";
    }

    public class NegativeSpeedModification : NegativeModification
    {
        public override string Name => "Stiff";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveSpeedModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Speed;
            float newValue = origValue - 2;
            Diff = origValue - newValue;
            item.BodyPartAspectDef.Speed = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff:F0} Speed";
    }

    public class PositiveSpeedModification : PositiveModification
    {
        public override string Name => "Red";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Speed;
            float newValue = origValue + 2;
            Diff = newValue - origValue;
            item.BodyPartAspectDef.Speed = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff:F0} Speed";
    }

    public class NegativePerceptionModification : NegativeModification
    {
        public override string Name => "Distracting";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositivePerceptionModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Perception;
            float newValue = origValue - 3;
            Diff = origValue - newValue;
            item.BodyPartAspectDef.Perception = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff:F0} Perception";
    }

    public class PositivePerceptionModification : PositiveModification
    {
        public override string Name => "Focussing";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Perception;
            float newValue = origValue + 3;
            Diff = newValue - origValue;
            item.BodyPartAspectDef.Perception = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff:F0} Perception";
    }

    public class NegativeStealthModification : NegativeModification
    {
        public override string Name => "Neon";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveStealthModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Stealth;
            float newValue = origValue - 0.1f;
            Diff = (origValue - newValue) * 100;
            item.BodyPartAspectDef.Stealth = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff:F0}% Stealth";
    }

    public class PositiveStealthModification : PositiveModification
    {
        public override string Name => "Camouflage";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Stealth;
            float newValue = origValue + 0.1f;
            Diff = (newValue - origValue) * 100;
            item.BodyPartAspectDef.Stealth = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff:F0}% Stealth";
    }

    public class NegativeAccuracyModification : NegativeModification
    {
        public override string Name => "Inaccurate";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveAccuracyModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Accuracy;
            float newValue = origValue - 0.05f;
            Diff = (origValue - newValue) * 100;
            item.BodyPartAspectDef.Accuracy = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff:F0}% Accuracy";
    }

    public class PositiveAccuracyModification : PositiveModification
    {
        public override string Name => "Accurate";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Accuracy;
            float newValue = origValue + 0.05f;
            Diff = (newValue - origValue) * 100;
            item.BodyPartAspectDef.Accuracy = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff:F0}% Accuracy";
    }


    public class PositiveWillpowerModification : PositiveModification
    {
        public override string Name => "Blessed";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.WillPower;
            Diff = 2f;
            float newValue = origValue + Diff;
            item.BodyPartAspectDef.WillPower = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff:F0} Willpower";
    }

    public class RegenerationAbilityModification : PositiveModification
    {
        public override string Name => "Regenerating";
        public override float SpawnWeightMultiplier => 0.25f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            // item.RequiredSlotBinds always contains exactly one element for all of the items I modify
            if (!new List<string> { "Human_Head_SlotDef", "Human_Torso_SlotDef", "Human_Legs_SlotDef" }.Contains(item.RequiredSlotBinds[0].RequiredSlot.name)) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            var abilitiesList = NewAbilities.GetItemAbilities(item);

            // make the ability depend on the item's slot and only apply to the relevant body part
            // Human_Head_SlotDef applies to Head
            if (item.RequiredSlotBinds[0].RequiredSlot.name == "Human_Head_SlotDef") abilitiesList.Add(NewAbilities.Regeneration_Head_Passive_AbilityDef);
            // Human_Legs_SlotDef applies to LeftLeg, RightLeg
            else if (item.RequiredSlotBinds[0].RequiredSlot.name == "Human_Legs_SlotDef") abilitiesList.Add(NewAbilities.Regeneration_Legs_Passive_AbilityDef);
            // Human_Torso_SlotDef applies to Torso, LeftArm, RightArm
            else if (item.RequiredSlotBinds[0].RequiredSlot.name == "Human_Torso_SlotDef")
            {
                ApplyStatusAbilityDef regenerationTorsoAbility = DefCache.GetDef<ApplyStatusAbilityDef>("Regeneration_Torso_Passive_AbilityDef");
                abilitiesList.Add(regenerationTorsoAbility);
            }

            item.Abilities = abilitiesList.ToArray();
        }
        public override string GetLocalizationDesc() => $"Regenerate 10 HP";
    }

    public class HighJumpAbilityModification : PositiveModification
    {
        public override string Name => "Leaping";
        public override float SpawnWeightMultiplier => 0.25f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Legs_SlotDef")) return true;  // for leg armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "Humanoid_HighJump_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Can jump one floor";
    }

    public class PoisonResistanceAbilityModification : PositiveModification
    {
        public override string Name => "Poison-Resistant";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Torso_SlotDef")) return true;  // for torso armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "PoisonResistant_DamageMultiplierAbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants poison resistance";
    }
    public class MotionDetectionAbilityModification : PositiveModification
    {
        public override string Name => "Motion-Detecting";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Head_SlotDef")) return true;  // for head armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "MotionDetection_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants motion detection";
    }
    public class MistRepellerAbilityModification : PositiveModification
    {
        public override string Name => "Anti-Mist";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Legs_SlotDef")) return true;  // for leg armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "MistRepeller_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Repels mist";
    }
    public class GooImmunityAbilityModification : PositiveModification
    {
        public override string Name => "Anti-Goo";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Legs_SlotDef")) return true;  // for leg armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "GooImmunity_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants goo immunity";
    }

    public class AcidResistanceAbilityModification : PositiveModification
    {
        public override string Name => "Acid-Resistant";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Torso_SlotDef")) return true;  // for torso armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "AcidResistant_DamageMultiplierAbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants acid resistance";
    }

    public class FireResistanceAbilityModification : PositiveModification
    {
        public override string Name => "Fire-Resistant";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Torso_SlotDef")) return true;  // for torso armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "FireResistant_DamageMultiplierAbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants fire resistance";
    }
    public class VirusResistanceAbilityModification : PositiveModification
    {
        public override string Name => "Virus-Resistant";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Head_SlotDef")) return true;  // for head armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "VirusResistant_DamageMultiplierAbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants virus resistance";
    }
    public class MindControlImmunityAbilityModification : PositiveModification
    {
        public override string Name => "Tinfoil";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Head_SlotDef")) return true;  // for head armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "MindControlImmunity_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants mind control immunity";
    }
    public class ShadowStepAbilityModification : PositiveModification
    {
        public override string Name => "Shadow Step";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!item.RequiredSlotBinds[0].RequiredSlot.name.Equals("Human_Legs_SlotDef")) return true;  // for leg armors only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "ShadowStep_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Grants shadow step";
    }

    public class TFTVAblativeVestResistanceAbilityModification : PositiveModification
    {
        public override string Name => "Plated";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "TFTV_AblativeVest_Resistance_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Ablative Plating";
    }

    public class TFTVAblativeVestHealthAbilityModification : PositiveModification
    {
        public override string Name => "Ablative";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "TFTV_AblativeVest_Health_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Increased bodypart HP";
    }

    public class TFTVHazmatVestResistanceAbilityModification : PositiveModification
    {
        public override string Name => "Hazmat";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "TFTV_HazmatVest_Resistance_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Hazard containment";
    }

    public class TFTVHazmatVestArmorAbilityModification : PositiveModification
    {
        public override string Name => "Reinforced";
        public override float SpawnWeightMultiplier => 0.1f;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            NewAbilities.AddAbilityToItem(item, "TFTV_HazmatVest_Armor_AbilityDef");
        }
        public override string GetLocalizationDesc() => $"Reinforced Seals";
    }
}
