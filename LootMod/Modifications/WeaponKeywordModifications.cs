﻿using System;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace LootMod.Modifications
{
    public class AddFireModification : PositiveModification
    {
        public override string Name => "Flaming";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageKeywords.Any(pair => pair.DamageKeywordDef == DefCache.keywords.BurningKeyword)) return true; // must not have the keyword yet
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            DamageKeywordPair preferredDamageKeywordPair = _getPreferredDamageKeyword(weapon);
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Ceiling(origValue * 0.2);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.BurningKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} fire damage";
    }

    public class AddPiercingModification : PositiveModification
    {
        public override string Name => "Piercing";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageKeywords.Any(pair => pair.DamageKeywordDef == DefCache.keywords.PiercingKeyword)) return true; // must not have the keyword yet
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            DamageKeywordPair preferredDamageKeywordPair = _getPreferredDamageKeyword(weapon);
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Ceiling(origValue * 0.3);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.PiercingKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} armor piercing";
    }

    public class AddShreddingModification : PositiveModification
    {
        public override string Name => "Shredding";
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
            float newValue = (float)Math.Ceiling(origValue * 0.1);
            // check if this weapon already has the ShreddingKeyword. If so, just increase its value. otherwise, add it. (almost all weapons have at last 1 shred)
            DamageKeywordPair shreddingDamageKeywordPair = weapon.DamagePayload.DamageKeywords.FirstOrDefault(pair => pair.DamageKeywordDef == DefCache.keywords.ShreddingKeyword);
            if (shreddingDamageKeywordPair != null)
            {
                newValue = shreddingDamageKeywordPair.Value + newValue;
                shreddingDamageKeywordPair.Value = newValue;
            }
            else weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.ShreddingKeyword, Value = newValue });
            Diff = newValue;

        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} shredding damage";
    }

    public class AddAcidModification : PositiveModification
    {
        public override string Name => "Acidic";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageKeywords.Any(pair => pair.DamageKeywordDef == DefCache.keywords.AcidKeyword)) return true; // must not have the keyword yet
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            DamageKeywordPair preferredDamageKeywordPair = _getPreferredDamageKeyword(weapon);
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Ceiling(origValue * 0.2);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.AcidKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} acid damage";
    }

    public class AddPoisonousModification : PositiveModification
    {
        public override string Name => "Poisonous";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageKeywords.Any(pair => pair.DamageKeywordDef == DefCache.keywords.PoisonousKeyword)) return true; // must not have the keyword yet
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            DamageKeywordPair preferredDamageKeywordPair = _getPreferredDamageKeyword(weapon);
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Ceiling(origValue * 0.1);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.PoisonousKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} poison damage";
    }

    public class AddViralModification : PositiveModification
    {
        public override string Name => "Viral";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageKeywords.Any(pair => pair.DamageKeywordDef == DefCache.keywords.ViralKeyword)) return true; // must not have the keyword yet
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            float totalProjectiles = weapon.DamagePayload.AutoFireShotCount * weapon.DamagePayload.ProjectilesPerShot;
            float newValue = (float)Math.Ceiling(6 / totalProjectiles);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.ViralKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} virus damage";
    }

    public class AddParalysingModification : PositiveModification
    {
        public override string Name => "Paralysing";
        public float Diff;
        public override float SpawnWeightMultiplier => 0.2f;

        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageKeywords.Any(pair => pair.DamageKeywordDef == DefCache.keywords.ParalysingKeyword)) return true; // must not have the keyword yet
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            float totalProjectiles = weapon.DamagePayload.AutoFireShotCount * weapon.DamagePayload.ProjectilesPerShot;
            float newValue = (float)Math.Ceiling(6 / totalProjectiles);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.ParalysingKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: +{Diff} paralysing damage";
    }
}
