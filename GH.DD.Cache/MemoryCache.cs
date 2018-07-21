using System.Collections.Concurrent;

namespace GH.DD.Cache
{
    // TODO: tests
    // TODO: comments
    // TODO: docs
    public class MemoryCache : ICache
    {
        private ConcurrentDictionary<object, ICacheEntry> data;
        private readonly object _locker = new object();
        
        public ICacheEntry Get(object key)
        {
            if (!data.TryGetValue(key, out var entry))
                return null;

            lock (_locker)
            {
                if (entry.IsExpired())
                    RemoveEntry(key);
            }

            return entry.IsAutoDeleted ? null : entry;
        }

        public void Set(object key, ICacheEntry entry)
        {
            data.AddOrUpdate(key, entry, (k, oldEntry) => entry);
        }

        private void RemoveEntry(object key)
        {
            if (!data.TryGetValue(key, out var entry))
                return;
            
            entry.ExecuteBeforeDeleteCallback();
            
            if (entry.IsAutoDeleted)
                data.TryRemove(key, out entry);
            
            entry.ExecuteAfterDeleteCallback();
        }
    }
}