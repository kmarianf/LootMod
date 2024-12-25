using System;
using System.Collections.Generic;
using System.Linq;
using Base.Entities.Abilities;
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
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.Armor;
            float newValue = (float)Math.Floor(item.Armor * 0.75);
            Diff = origValue - newValue;
            item.Armor = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff:F0} Armor";
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
        public override string GetLocalozationDesc() => $"{Name}: +{Diff:F0} Armor";
    }

    public class NegativeSpeedModification : NegativeModification
    {
        public override string Name => "Slowed";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveSpeedModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Speed;
            float newValue = origValue - 2;
            Diff = origValue - newValue;
            item.BodyPartAspectDef.Speed = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff:F0} Speed";
    }

    public class PositiveSpeedModification : PositiveModification
    {
        public override string Name => "Hasted";
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
        public override string GetLocalozationDesc() => $"{Name}: +{Diff:F0} Speed";
    }

    public class NegativePerceptionModification : NegativeModification
    {
        public override string Name => "Distracting";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositivePerceptionModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Perception;
            float newValue = origValue - 2;
            Diff = origValue - newValue;
            item.BodyPartAspectDef.Perception = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff:F0} Perception";
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
            float newValue = origValue + 2;
            Diff = newValue - origValue;
            item.BodyPartAspectDef.Perception = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff:F0} Perception";
    }

    public class NegativeStealthModification : NegativeModification
    {
        public override string Name => "Neon";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveStealthModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Stealth;
            float newValue = origValue - 0.1f;
            Diff = (origValue - newValue) * 100;
            item.BodyPartAspectDef.Stealth = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff:F0}% Stealth";
    }

    public class PositiveStealthModification : PositiveModification
    {
        public override string Name => "Camoflage";
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
        public override string GetLocalozationDesc() => $"{Name}: +{Diff:F0}% Stealth";
    }

    public class NegativeAccuracyModification : NegativeModification
    {
        public override string Name => "Inaccurate";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveAccuracyModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Accuracy;
            float newValue = origValue - 0.04f;
            Diff = (origValue - newValue) * 100;
            item.BodyPartAspectDef.Accuracy = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff:F0}% Accuracy";
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
            float newValue = origValue + 0.04f;
            Diff = (newValue - origValue) * 100;
            item.BodyPartAspectDef.Accuracy = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff:F0}% Accuracy";
    }

    public class RegenrationAbilityModification : PositiveModification
    {
        public override string Name => "Regenerating";
        public override float SpawnWeightMultiplier => 10f;
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (item is WeaponDef) return true;  // for non-weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            var abilities = item.Abilities;
            List<AbilityDef> abilitiesList;
            if (abilities == null) abilitiesList = new List<AbilityDef>();
            else abilitiesList = abilities.ToList();
            ApplyStatusAbilityDef regenerationAbility = DefCache.GetDef<ApplyStatusAbilityDef>("Regeneration_Torso_Passive_AbilityDef");
            abilitiesList.Add(regenerationAbility);
            item.Abilities = abilitiesList.ToArray();
        }
        public override string GetLocalozationDesc() => $"{Name}: Regenrate 10";
    }
}
