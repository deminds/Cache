using System.Collections.Concurrent;

namespace GH.DD.Cache
{
    /// <summary>
    /// Object of cache placed in RAM
    /// </summary>
    public class MemoryCache : ICache
    {
        private ConcurrentDictionary<string, ICacheEntry> data = new ConcurrentDictionary<string, ICacheEntry>();
        private readonly object _locker = new object();
        
        /// <inheritdoc cref="ICache.Get"/>
        public ICacheEntry Get(string key)
        {
            if (!data.TryGetValue(key, out var entry))
                return null;

            lock (_locker)
            {
                if (entry.IsExpired())
                {
                    RemoveEntry(key);
                    return entry.IsAutoDeleted ? null : entry;
                }
            }

            return entry;
        }

        /// <inheritdoc cref="ICache.Set"/>
        public void Set(string key, ICacheEntry entry)
        {
            data.AddOrUpdate(key, entry, (k, oldEntry) => entry);
        }

        private void RemoveEntry(string key)
        {
            if (!data.TryGetValue(key, out var entry))
                return;
            
            entry.ExecuteUpdateDataCallback();
            
            if (entry.IsAutoDeleted)
                data.TryRemove(key, out entry);
        }
    }
}