using System;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace LootMod
{
    public class InvalidModificationException : Exception
    {
        public InvalidModificationException(string message) : base(message) { }
    }

    public abstract class BaseModification
    {
        public abstract string Name { get; }  // the name that will show up in the hame. editing this will not corrupt save files.
        public virtual string modificationId => this.GetType().Name.Replace("Modification", "");  // used in eg the localization file. editing this will corrupt save files.
        /// <summary>higher multiplier = higher SpawnWeight, so strong modifications get a low multiplier</summary>
        public virtual float SpawnWeightMultiplier => 1f;
        /// <summary>some weapons have non or multiple of these. we prefer them in this order.</summary>
        public List<DamageKeywordDef> preferredDmgKeywords = new List<DamageKeywordDef> {
            DefCache.keywords.DamageKeyword,
            DefCache.keywords.BlastKeyword
        };
        public abstract void AddModification(TacticalItemDef item);
        public virtual string EditLocalozationName(string localozationName)
        {
            localozationName = $"{Name} {localozationName}";
            return localozationName;
        }
        /// <summary>return true if the modification and the item or the combination of modifications are invlaid</summary>
        public virtual bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination) => false;
        public abstract string GetLocalozationDesc();
    }

    public abstract class PositiveModification : BaseModification
    {
        public override float SpawnWeightMultiplier => 0.5f;
    }
    public abstract class NegativeModification : BaseModification
    {
        public override float SpawnWeightMultiplier => 2f;
    }

    public class NegativeWeightModification : NegativeModification
    {
        public override string Name => "Heavy";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            List<Type> excludedMods = new List<Type> { typeof(PositiveWeightModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void AddModification(TacticalItemDef item)
        {
            int origValue = item.Weight;
            int newValue = (int)Math.Ceiling((float)item.Weight * 1.5);
            Diff = newValue - origValue;
            item.Weight = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} weight";
    }

    public class PositiveWeightModification : PositiveModification
    {
        public override string Name => "Light";
        public int Diff;
        public override void AddModification(TacticalItemDef item)
        {
            int origValue = item.Weight;
            int newValue = (int)Math.Floor((float)item.Weight * 0.33);
            Diff = origValue - newValue;
            item.Weight = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff} weight";
    }

    public class NegativeDamageModification : NegativeModification
    {
        public override string Name => "Weak";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            List<Type> excludedMods = new List<Type> { typeof(PositiveDamageModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void AddModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            // find the DamageKeywordPair with the first preferred Damage_DamageKeywordDataDef within weapon.DamagePayload.DamageKeywords. edit that value.
            DamageKeywordPair preferredDamageKeywordPair = null;
            foreach (DamageKeywordDef preferredDmgKeyword in preferredDmgKeywords)
            {
                preferredDamageKeywordPair = weapon.DamagePayload.DamageKeywords.FirstOrDefault(pair => pair.DamageKeywordDef == preferredDmgKeyword);
                if (preferredDamageKeywordPair != null) break;
            }
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Floor(origValue * 0.75);
            Diff = origValue - newValue;
            preferredDamageKeywordPair.Value = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff} damage";
    }

    public class PositiveDamageModification : PositiveModification
    {
        public override string Name => "Deadly";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void AddModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            // find the DamageKeywordPair with the first preferred Damage_DamageKeywordDataDef within weapon.DamagePayload.DamageKeywords. edit that value.
            DamageKeywordPair preferredDamageKeywordPair = null;
            foreach (DamageKeywordDef preferredDmgKeyword in preferredDmgKeywords)
            {
                preferredDamageKeywordPair = weapon.DamagePayload.DamageKeywords.FirstOrDefault(pair => pair.DamageKeywordDef == preferredDmgKeyword);
                if (preferredDamageKeywordPair != null) break;
            }
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Ceiling(origValue * 1.25);
            Diff = newValue - origValue;
            preferredDamageKeywordPair.Value = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} damage";
    }

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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
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
        public override void AddModification(TacticalItemDef item)
        {
            float origValue = item.BodyPartAspectDef.Accuracy;
            float newValue = origValue + 0.04f;
            Diff = (newValue - origValue) * 100;
            item.BodyPartAspectDef.Accuracy = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff:F0}% Accuracy";
    }
}
