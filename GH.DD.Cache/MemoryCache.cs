using System.Collections.Generic;

namespace GH.DD.Cache
{
    public class MemoryCache : ICache
    {
        private Dictionary<string, ICacheEntry> data;
        private object _locker = new object();
        
        public ICacheEntry Get(string key)
        {
            if (!data.ContainsKey(key))
                return null;

            var entry = data[key];

            if (entry.IsExpired())
                RemoveEntry(key);

            return entry.IsAutoDeleted ? null : entry;
        }

        public void Set(string key, ICacheEntry entry)
        {
            data.Add(key, entry);
        }

        private void RemoveEntry(string key)
        {
            if (!data.ContainsKey(key))
                return;

            var entry = data[key];
            
            entry.ExecuteBeforeDeleteCallback();
            if (entry.IsAutoDeleted) data.Remove(key);
            entry.ExecuteAfterDeleteCallback();
        }
    }
}