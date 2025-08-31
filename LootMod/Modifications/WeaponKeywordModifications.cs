using System;
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

            //often starts fires that deal way more damage than the weapon itself. so just 1 damage is already strong.
            //float newValue = (float)Math.Ceiling(origValue * 0.1);
            float newValue = 1;

            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.BurningKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalizationDesc() => $"starts fires";
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
        public override string GetLocalizationDesc() => $"+{Diff} armor piercing";
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
            float APToUse = weapon.APToUsePerc / 25;
            float totalProjectiles = weapon.DamagePayload.AutoFireShotCount * weapon.DamagePayload.ProjectilesPerShot;
            float newValue = (float)Math.Ceiling((5 * APToUse) / totalProjectiles);
            Diff = newValue;
            // check if this weapon already has the ShreddingKeyword. If so, just increase its value. otherwise, add it. (almost all weapons have at last 1 shred)
            DamageKeywordPair shreddingDamageKeywordPair = weapon.DamagePayload.DamageKeywords.FirstOrDefault(pair => pair.DamageKeywordDef == DefCache.keywords.ShreddingKeyword);
            if (shreddingDamageKeywordPair != null)
            {
                newValue = shreddingDamageKeywordPair.Value + newValue;
                shreddingDamageKeywordPair.Value = newValue;
            }
            else weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.ShreddingKeyword, Value = newValue });

        }
        public override string GetLocalizationDesc() => $"Shreds +{Diff} armor";
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
        public override string GetLocalizationDesc() => $"+{Diff} acid damage";
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
            float newValue = (float)Math.Ceiling(origValue * 0.2);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.PoisonousKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff} poison damage";
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
            float APToUse = weapon.APToUsePerc / 25;
            float totalProjectiles = weapon.DamagePayload.AutoFireShotCount * weapon.DamagePayload.ProjectilesPerShot;
            float newValue = (float)Math.Ceiling((2 + 2 * APToUse) / totalProjectiles);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.ViralKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff} virus damage";
    }

    public class AddParalysingModification : PositiveModification
    {
        public override string Name => "Paralyzing";
        public float Diff;

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
            float APToUse = weapon.APToUsePerc / 25;
            float totalProjectiles = weapon.DamagePayload.AutoFireShotCount * weapon.DamagePayload.ProjectilesPerShot;
            float newValue = (float)Math.Ceiling((3 + 3 * APToUse) / totalProjectiles);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.ParalysingKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff} paralysis damage";
    }
    public class AddBleedingModification : PositiveModification
    {
        public override string Name => "Slashing";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageKeywords.Any(pair => pair.DamageKeywordDef == DefCache.keywords.BleedingKeyword)) return true; // must not have the keyword yet
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            DamageKeywordPair preferredDamageKeywordPair = _getPreferredDamageKeyword(weapon);
            float origValue = preferredDamageKeywordPair.Value;
            float newValue = (float)Math.Ceiling(origValue * 0.15);
            weapon.DamagePayload.DamageKeywords.Add(new DamageKeywordPair() { DamageKeywordDef = DefCache.keywords.BleedingKeyword, Value = newValue });
            Diff = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff} bleeding damage";
    }


    public class PositiveBodyPartDamageModification : PositiveModification
    {
        public override string Name => "Disabling";
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
            float origValue = weapon.DamagePayload.BodyPartMultiplier;
            float newValue = (float)Math.Ceiling(origValue * 2f);
            weapon.DamagePayload.BodyPartMultiplier = newValue;
            Diff = newValue / origValue;
        }
        public override string GetLocalizationDesc() => $"x{Diff} body part damage";
    }
}
