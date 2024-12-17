using System;
using System.Linq;
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
        public abstract void AddModification(TacticalItemDef item);
        public string EditLocalozationName(string localozationName)
        {
            localozationName = $"{Name} {localozationName}";
            return localozationName;
        }
        public abstract string GetLocalozationDesc();
    }

    public abstract class PositiveModification : BaseModification { }
    public abstract class NegativeModification : BaseModification { }

    public class BulkyModification : NegativeModification
    {
        public override string Name => "Bulky";
        public override int Rarity => -1;
        public int Diff;
        public override void AddModification(TacticalItemDef item)
        {
            int origValue = item.Weight;
            int newValue = (int)Math.Ceiling((float)item.Weight * 1.33);
            Diff = newValue - origValue;
            item.Weight = newValue;
        }
        public override string GetLocalozationDesc()
        {
            return $"{Name}: +{Diff} weight";
        }
    }

    public class WeakModification : NegativeModification
    {
        public override string Name => "Weak";
        public override int Rarity => -1;
        public float Diff;
        public override void AddModification(TacticalItemDef item)
        {
            // we expect only int for damage, but for some reason the games handles it as float
            // DamagePayload.DamageKeywords are prefered over the DamagePayload.DamageValue. TODO find for which weapons this isnt the case!
            if (item is WeaponDef weapon)
            {
                // find the Damage_DamageKeywordDataDef within weapon.DamagePayload.DamageKeywords, edit that
                var damageKeywordPair = weapon.DamagePayload.DamageKeywords.FirstOrDefault(pair => pair.DamageKeywordDef == DefCache.keywords.DamageKeyword);
                if (damageKeywordPair != null)
                {
                    float origValue = damageKeywordPair.Value;
                    float newValue = (float)Math.Floor(origValue * 0.75);
                    Diff = origValue - newValue;
                    damageKeywordPair.Value = newValue;
                }
                else { throw new InvalidModificationException("weapon has no normal damage keyword."); }
            }
            else { throw new InvalidModificationException("item is not a weapon."); }
        }
        public override string GetLocalozationDesc()
        {
            return $"{Name}: -{Diff} damage";
        }
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
        public override string GetLocalozationDesc()
        {
            return $"{Name}: -{Diff} weight";
        }
    }

    public class DeadlyModification : PositiveModification
    {
        public override string Name => "Deadly";
        public override int Rarity => 1;
        public float Diff;
        public override void AddModification(TacticalItemDef item)
        {
            // we expect only int for damage, but for some reason the games handles it as float
            // DamagePayload.DamageKeywords are prefered over the DamagePayload.DamageValue. TODO find for which weapons this isnt the case!
            if (item is WeaponDef weapon)
            {
                // find the normal DamageKeyword within weapon.DamagePayload.DamageKeywords, edit that
                var damageKeywordPair = weapon.DamagePayload.DamageKeywords.FirstOrDefault(pair => pair.DamageKeywordDef == DefCache.keywords.DamageKeyword);
                if (damageKeywordPair != null)
                {
                    float origValue = damageKeywordPair.Value;
                    float newValue = (float)Math.Ceiling(origValue * 1.25);
                    Diff = newValue - origValue;
                    damageKeywordPair.Value = newValue;
                }
                else { throw new InvalidModificationException("weapon has no normal damage keyword."); }
            }
            else { throw new InvalidModificationException("item is not a weapon."); }
        }
        public override string GetLocalozationDesc()
        {
            return $"{Name}: +{Diff} damage";
        }
    }
}
