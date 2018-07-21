using System.Collections.Generic;

namespace GH.DD.Cache
{
    // TODO: tests
    // TODO: comments
    // TODO: docs
    public class MemoryCache : ICache
    {
        // TODO: to ConcurentDictionary
        private Dictionary<object, ICacheEntry> data;
        private object _locker = new object();
        
        public ICacheEntry Get(object key)
        {
            if (!data.ContainsKey(key))
                return null;

            var entry = data[key];

            if (entry.IsExpired())
                RemoveEntry(key);

            return entry.IsAutoDeleted ? null : entry;
        }

        public void Set(object key, ICacheEntry entry)
        {
            data.Add(key, entry);
        }

        private void RemoveEntry(object key)
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