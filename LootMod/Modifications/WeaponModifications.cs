using System;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace LootMod.Modifications
{
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
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            DamageKeywordPair preferredDamageKeywordPair = _getPreferredDamageKeyword(weapon);
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
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            DamageKeywordPair preferredDamageKeywordPair = _getPreferredDamageKeyword(weapon);
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Ceiling(origValue * 1.25);
            Diff = newValue - origValue;
            preferredDamageKeywordPair.Value = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} damage";
    }

    public class NegativeShotCountModification : NegativeModification
    {
        public override string Name => "Low-Burst";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.AutoFireShotCount <= 1) return true;  // not valid for single shot weapons
            List<Type> excludedMods = new List<Type> { typeof(PositiveShotCountModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.DamagePayload.AutoFireShotCount;
            int newValue = Math.Max(1, (int)Math.Floor(origValue * 0.8));
            Diff = origValue - newValue;
            weapon.DamagePayload.AutoFireShotCount = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff} burst shots";
    }

    public class PositiveShotCountModification : PositiveModification
    {
        public override string Name => "High-Burst";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.AutoFireShotCount <= 1) return true;  // not valid for single shot weapons
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.DamagePayload.AutoFireShotCount;
            int newValue = (int)Math.Ceiling(origValue * 1.2);
            Diff = newValue - origValue;
            weapon.DamagePayload.AutoFireShotCount = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} burst shots";
    }
    public class NegativeProjectilesPerShotModification : NegativeModification
    {
        public override string Name => "Low-Burst";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.ProjectilesPerShot <= 1) return true;  // not valid for single projectile weapons
            List<Type> excludedMods = new List<Type> { typeof(PositiveProjectilesPerShotModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.DamagePayload.ProjectilesPerShot;
            int newValue = Math.Max(1, (int)Math.Floor(origValue * 0.8));
            Diff = origValue - newValue;
            weapon.DamagePayload.ProjectilesPerShot = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff} projectiles per shot";
    }

    public class PositiveProjectilesPerShotModification : PositiveModification
    {
        public override string Name => "High-Burst";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.ProjectilesPerShot <= 1) return true;  // not valid for single projectile weapons
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.DamagePayload.ProjectilesPerShot;
            int newValue = (int)Math.Ceiling(origValue * 1.2);
            Diff = newValue - origValue;
            weapon.DamagePayload.ProjectilesPerShot = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} projectiles per shot";
    }




}
