
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities.DamageKeywords;


namespace LootMod
{
    public static class DefCache
    {
        private static readonly DefRepository _repo = GameUtl.GameComponent<DefRepository>();
        internal static readonly SharedDamageKeywordsDataDef keywords = GameUtl.GameComponent<SharedData>().SharedDamageKeywords;
        private static readonly Dictionary<string, List<string>> _defNameToGuidCache;

        static DefCache()
        {
            _defNameToGuidCache = new Dictionary<string, List<string>>();
            RecalculateCache();
        }
        public static DefRepository Repo => _repo;

        public static void RecalculateCache()
        {
            _defNameToGuidCache.Clear();
            foreach (BaseDef baseDef in _repo.DefRepositoryDef.AllDefs)
            {
                AddDef(baseDef.name, baseDef.Guid);
            }
        }

        public static T GetDef<T>(string name) where T : BaseDef
        {
            if (!_defNameToGuidCache.ContainsKey(name))
            {
                return null;
            }
            string guid = _defNameToGuidCache[name].FirstOrDefault();
            return guid != default ? (T)_repo.GetDef(guid) : null;
        }

        public static BaseDef GetDef(string name)
        {
            if (!_defNameToGuidCache.ContainsKey(name))
            {
                return null;
            }
            string guid = _defNameToGuidCache[name].FirstOrDefault();
            return guid != default ? _repo.GetDef(guid) : null;
        }

        public static void AddDef(string name, string guid)
        {
            if (_defNameToGuidCache.ContainsKey(name))
            {
                if (!_defNameToGuidCache[name].Contains(guid))
                {
                    _defNameToGuidCache[name].Add(guid);
                }
            }
            else
            {
                _defNameToGuidCache.Add(name, new List<string> { guid });
            }
        }
    }
}
