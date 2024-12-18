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
        public abstract string Name { get; }
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

    public class BulkyModification : NegativeModification
    {
        public override string Name => "Bulky";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            List<Type> excludedMods = new List<Type> { typeof(SlimModification) };
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

    public class WeakModification : NegativeModification
    {
        public override string Name => "Weak";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            List<Type> excludedMods = new List<Type> { typeof(DeadlyModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
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
            float newValue = (float)Math.Floor(origValue * 0.75);
            Diff = origValue - newValue;
            preferredDamageKeywordPair.Value = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff} damage";
    }

    public class SlimModification : PositiveModification
    {
        public override string Name => "Slim";
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

    public class DeadlyModification : PositiveModification
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
}
