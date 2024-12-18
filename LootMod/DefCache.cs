
using System.Collections.Generic;
using System.Linq;
using Base.Core;
using Base.Defs;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Tactical.Entities.DamageKeywords;


namespace LootMod
{
    public class DefCache
    {
        private readonly DefRepository _repo = GameUtl.GameComponent<DefRepository>();
        internal static SharedDamageKeywordsDataDef keywords = GameUtl.GameComponent<SharedData>().SharedDamageKeywords;
        public DefRepository Repo => _repo;
        private Dictionary<string, List<string>> _defNameToGuidCache;
        public DefCache()
        {
            _defNameToGuidCache = new Dictionary<string, List<string>>();
            foreach (BaseDef baseDef in _repo.DefRepositoryDef.AllDefs)
            {
                AddDef(baseDef.name, baseDef.Guid);
            }
        }


        public T GetDef<T>(string name) where T : BaseDef
        {
            if (!_defNameToGuidCache.ContainsKey(name))
            {
                return null;
            }
            string guid = _defNameToGuidCache[name].FirstOrDefault();
            return guid != default ? (T)_repo.GetDef(guid) : null;
        }

        public BaseDef GetDef(string name)
        {
            if (!_defNameToGuidCache.ContainsKey(name))
            {
                return null;
            }
            string guid = _defNameToGuidCache[name].FirstOrDefault();
            return guid != default ? _repo.GetDef(guid) : null;
        }


        public void AddDef(string name, string guid)
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
