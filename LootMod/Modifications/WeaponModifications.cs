using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LootMod.Modifications.Abilities;
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
            if (!weapon.DamagePayload.DamageKeywords.Any(pair => preferredDmgKeywords.Contains(pair.DamageKeywordDef))) return true; // must have any DamageKeyword
            List<Type> excludedMods = new List<Type> { typeof(PositiveDamageModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
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
        public override string GetLocalizationDesc() => $"-{Diff} damage";
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
        public override string GetLocalizationDesc() => $"+{Diff} damage";
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
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.DamagePayload.AutoFireShotCount;
            int newValue = Math.Max(1, (int)Math.Floor(origValue * 0.8));
            Diff = origValue - newValue;
            weapon.DamagePayload.AutoFireShotCount = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff} burst shots";
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
        public override string GetLocalizationDesc() => $"+{Diff} burst shots";
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
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.DamagePayload.ProjectilesPerShot;
            int newValue = Math.Max(1, (int)Math.Floor(origValue * 0.8));
            Diff = origValue - newValue;
            weapon.DamagePayload.ProjectilesPerShot = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff} projectiles per shot";
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
        public override string GetLocalizationDesc() => $"+{Diff} projectiles per shot";
    }

    public class NegativeRangeModification : NegativeModification
    {
        public override string Name => "Inaccurate";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageDeliveryType != PhoenixPoint.Tactical.Entities.DamageDeliveryType.DirectLine) return true;  // only valid for direct line weapons
            List<Type> excludedMods = new List<Type> { typeof(PositiveRangeModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.EffectiveRange;
            weapon.SpreadDegrees = weapon.SpreadDegrees * 1.4f;
            // reset the EffectiveRange to -1 to force recalculation. this is a private set, thus the workaround via reflection.
            typeof(WeaponDef).GetField("_effectiveRange", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(weapon, -1);
            int newValue = weapon.EffectiveRange;
            Diff = origValue - newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff:F0} effective range";
    }
    public class PositiveRangeModification : PositiveModification
    {
        public override string Name => "Accurate";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            WeaponDef weapon = (WeaponDef)item;
            if (weapon.DamagePayload.DamageDeliveryType != PhoenixPoint.Tactical.Entities.DamageDeliveryType.DirectLine) return true;
            List<Type> excludedMods = new List<Type> { typeof(NegativeRangeModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            WeaponDef weapon = (WeaponDef)item;
            int origValue = weapon.EffectiveRange;
            weapon.SpreadDegrees = weapon.SpreadDegrees * 0.6f;
            // reset the EffectiveRange to -1 to force recalculation. this is a private set, thus the workaround via reflection.
            typeof(WeaponDef).GetField("_effectiveRange", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(weapon, -1);
            int newValue = weapon.EffectiveRange;
            Diff = newValue - origValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff:F0} effective range";
    }

    public class NegativeHitPointsModification : NegativeModification
    {
        public override string Name => "Fragile";
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            List<Type> excludedMods = new List<Type> { typeof(PositiveHitPointsModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            item.HitPoints = (float)Math.Floor(item.HitPoints * 0.10);
        }
        public override string GetLocalizationDesc() => $"breaks quickly";
    }

    public class PositiveHitPointsModification : PositiveModification
    {
        public override string Name => "Durable";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            item.HitPoints = (float)Math.Floor(item.HitPoints * 10);
            item.DestroyOnActorDeathPerc = 0;
        }
        public override string GetLocalizationDesc() => $"almost indestrucible";
    }

    public class AmmoPrinterModification : PositiveModification
    {
        public override string Name => "Ammo-Printing";
        public float Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            if (item.ChargesMax <= 0) return true;  // make sure it has charges (=ammo capacity)
            if (item.DestroyAtZeroCharges == true) return true;  // not valid for weapons that are destroyed at zero charges
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            if (item is EquipmentDef equipment)
            {
                equipment.FreeReloadOnMissionEnd = true;
                // remove the Reload_AbilityDef
                var abilities = NewAbilities.GetItemAbilities(equipment);
                var reloadAbility = abilities.FirstOrDefault(ability => ability.name == "Reload_AbilityDef");
                if (reloadAbility != null) abilities.Remove(reloadAbility);
                equipment.Abilities = abilities.ToArray();
                // remove compatible ammo
                equipment.CompatibleAmmunition = new TacticalItemDef[0];
            }
        }
        public override string GetLocalizationDesc() => $"Refills its ammo between missions. Cannot be reloaded";
    }

    public class NegativeAmmoModification : NegativeModification
    {
        public override float SpawnWeightMultiplier => 10f;
        public override string Name => "Low-Capacity";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            if (item.ChargesMax <= 1) return true;  // make sure it has charges, and more than one (=ammo capacity)
            if (item.DestroyAtZeroCharges == true) return true;  // not valid for weapons that are destroyed at zero charges

            // Only valid when combined with AmmoPrinterModification, because reloading a weapon with this mod destroys the magazine without reloading it
            List<Type> includedMods = new List<Type> { typeof(AmmoPrinterModification) };
            if (!combination.Any(modification => includedMods.Contains(modification.GetType()))) return true;
            
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            int origValue = item.ChargesMax;
            int newValue = (int)Math.Ceiling(item.ChargesMax * 0.5f);
            Diff = origValue - newValue;
            item.ChargesMax = newValue;
        }
        public override string GetLocalizationDesc() => $"-{Diff} magazine capacity";
    }

    public class PositiveAmmoModification : PositiveModification
    {
        public override string Name => "High-Capacity";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            if (!(item is WeaponDef)) return true;  // for weapons only
            if (item.ChargesMax <= 0) return true;  // make sure it has charges (=ammo capacity)
            if (item.DestroyAtZeroCharges == true) return true;  // not valid for weapons that are destroyed at zero charges
            List<Type> excludedMods = new List<Type> { typeof(NegativeAmmoModification) };
            if (combination.Any(modification => excludedMods.Contains(modification.GetType()))) return true;
            return false;
        }
        public override void ApplyModification(TacticalItemDef item)
        {
            int origValue = item.ChargesMax;
            int newValue = (int)Math.Ceiling(item.ChargesMax * 2.0f);  // I think this must be a int multiple of the original value, is buggy otherwise
            Diff = newValue - origValue;
            item.ChargesMax = newValue;
        }
        public override string GetLocalizationDesc() => $"+{Diff} magazine capacity";
    }

}
