using System.Collections.Concurrent;

namespace GH.DD.Cache
{
    // TODO: tests
    // TODO: readme
    /// <summary>
    /// Object of cache placed in RAM
    /// </summary>
    public class MemoryCache : ICache
    {
        private ConcurrentDictionary<object, ICacheEntry> data = new ConcurrentDictionary<object, ICacheEntry>();
        private readonly object _locker = new object();
        
        /// <inheritdoc cref="ICache.Get"/>
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

        /// <inheritdoc cref="ICache.Set"/>
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