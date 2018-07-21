using System;

namespace GH.DD.Cache
{
    public static class CacheExtensions
    {
        public static TItem Get<TItem>(this ICache cache, object key)
        {
            var entry = cache.Get(key);
            if (entry == null)
                return default(TItem);

            return (TItem) entry;
        }

        public static bool TryGet(this ICache cache, object key, out object value)
        {
            var entry = cache.Get(key);
            if (entry == null)
            {
                value = null;
                return false;
            }

            value = entry;
            return true;
        }

        public static bool TryGet<TItem>(this ICache cache, object key, out TItem value)
        {
            var entry = cache.Get(key);
            if (entry == null)
            {
                value = default(TItem);
                return false;
            }

            value = (TItem) entry;
            return true;
        }

        public static void Set<TItem>(this ICache cache, object key, TItem value)
        {
            var entry = new CacheEntry(key, value, new CacheEntryOptions());
            cache.Set(key, entry);
        }

        public static void Set<TItem>(this ICache cache, object key, TItem value, TimeSpan ttl)
        {
            var entryOptions = new CacheEntryOptions()
            {
                Ttl = ttl
            };
            
            var entry = new CacheEntry(key, value, entryOptions);
            
            cache.Set(key, entry);
        }
        
        public static void Set<TItem>(this ICache cache, object key, TItem value, TimeSpan ttl,
            BeforeDeleteDelegate beforeDeleteDelegate, bool isAutoDeleted = true)
        {
            var entryOptions = new CacheEntryOptions()
            {
                Ttl = ttl,
                BeforeDeleteCallback = beforeDeleteDelegate,
                IsAutoDeleted = isAutoDeleted
            };
            
            var entry = new CacheEntry(key, value, entryOptions);
            
            cache.Set(key, entry);
        }

        public static TItem GetOrCreate<TItem>(this ICache cache, object key, Func<TItem> fabric, CacheEntryOptions options)
        {
            if (cache.TryGet<TItem>(key, out var value))
                return value;

            value = fabric();
            var entry = new CacheEntry(key, value, options);
            
            return value;
        }
        
        public static TItem GetOrCreate<TItem>(this ICache cache, object key, Func<TItem> fabric,
            TimeSpan ttl)
        {
            var options = new CacheEntryOptions()
            {
                Ttl = ttl
            };
            
            return cache.GetOrCreate(key, fabric, options);
        }

        public static TItem GetOrCreate<TItem>(this ICache cache, object key, Func<TItem> fabric,
            TimeSpan ttl, BeforeDeleteDelegate beforeDeleteCallback, bool isAutoDeleted = true)
        {
            var options = new CacheEntryOptions()
            {
                Ttl = ttl,
                BeforeDeleteCallback = beforeDeleteCallback,
                IsAutoDeleted = isAutoDeleted
            };
            
            return cache.GetOrCreate(key, fabric, options);
        }
    }
}