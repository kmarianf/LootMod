using System;
using System.Collections.Generic;
using System.Linq;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Weapons;

namespace LootMod.Modifications
{
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
        public abstract void ApplyModification(TacticalItemDef item);
        public virtual string EditLocalozationName(string localozationName)
        {
            localozationName = $"{Name} {localozationName}";
            return localozationName;
        }
        /// <summary>return true if the modification and the item or the combination of modifications are invlaid</summary>
        public virtual bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination) => false;
        public abstract string GetLocalozationDesc();
        /// <summary>find the DamageKeywordPair with the first preferred Damage_DamageKeywordDataDef within weapon.DamagePayload.DamageKeywords</summary>
        internal virtual DamageKeywordPair _getPreferredDamageKeyword(WeaponDef weapon)
        {
            DamageKeywordPair preferredDamageKeywordPair = null;
            foreach (DamageKeywordDef preferredDmgKeyword in preferredDmgKeywords)
            {
                preferredDamageKeywordPair = weapon.DamagePayload.DamageKeywords.FirstOrDefault(pair => pair.DamageKeywordDef == preferredDmgKeyword);
                if (preferredDamageKeywordPair != null) break;
            }
            return preferredDamageKeywordPair;
        }
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
        public override string Name => "Encumbering";
        public int Diff;
        public override bool IsModificationOrComboInvalid(TacticalItemDef item, List<BaseModification> combination)
        {
            List<Type> excludedMods = new List<Type> { typeof(PositiveWeightModification) };
            return combination.Any(modification => excludedMods.Contains(modification.GetType()));
        }
        public override void ApplyModification(TacticalItemDef item)
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
        public override void ApplyModification(TacticalItemDef item)
        {
            int origValue = item.Weight;
            int newValue = (int)Math.Floor((float)item.Weight * 0.33);
            Diff = origValue - newValue;
            item.Weight = newValue;
        }
        public override string GetLocalozationDesc() => $"{Name}: -{Diff} weight";
    }


}
