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
        public abstract int Rarity { get; }
        // some weapons have non or multiple of these. we prefer them in this order.
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

    public abstract class PositiveModification : BaseModification { }
    public abstract class NegativeModification : BaseModification { }

    public class BulkyModification : NegativeModification
    {
        public override string Name => "Bulky";
        public override int Rarity => -1;
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            List<Type> excludedMods = new List<Type> { typeof(SlimModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void AddModification(TacticalItemDef item)
        {
            int origValue = item.Weight;
            int newValue = (int)Math.Ceiling((float)item.Weight * 1.34);
            Diff = newValue - origValue;
            item.Weight = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} weight";
    }

    public class WeakModification : NegativeModification
    {
        public override string Name => "Weak";
        public override int Rarity => -1;
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
        public override int Rarity => 1;
        public int Diff;
        public override void AddModification(TacticalItemDef item)
        {
            int origValue = item.Weight;
            int newValue = (int)Math.Floor((float)item.Weight * 0.34);
            Diff = origValue - newValue;
            item.Weight = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff} weight";
    }

    public class DeadlyModification : PositiveModification
    {
        public override string Name => "Deadly";
        public override int Rarity => 1;
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
