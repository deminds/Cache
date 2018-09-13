using System;

namespace GH.DD.Cache
{
    /// <summary>
    /// Extensions for easy manage Cache
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Get value from Cache
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Cache key</param>
        /// <typeparam name="TItem">Cast value to this Type</typeparam>
        /// <returns>Value of <see cref="CacheEntry.Value"/>. If key not found will return Null</returns>
        public static TItem Get<TItem>(this ICache cache, string key)
        {
            var entry = cache.Get(key);
            if (entry == null)
                return default(TItem);

            return (TItem) entry.Value;
        }

        /// <summary>
        /// Try get value from Cache
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        /// <returns>True if cache contains key</returns>
        public static bool TryGet(this ICache cache, string key, out object value)
        {
            var entry = cache.Get(key);
            if (entry == null)
            {
                value = null;
                return false;
            }

            value = entry.Value;
            return true;
        }

        /// <inheritdoc cref="TryGet"/>
        /// <typeparam name="TItem">Cast value to this Type</typeparam>
        public static bool TryGet<TItem>(this ICache cache, string key, out TItem value)
        {
            var entry = cache.Get(key);
            if (entry == null)
            {
                value = default(TItem);
                return false;
            }

            value = (TItem) entry.Value;
            return true;
        }

        /// <summary>
        /// Create or update cache entry with default values of <see cref="CacheEntryOptions"/>
        /// Will never expired. Updated only manually
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        public static void Set(this ICache cache, string key, object value)
        {
            var entry = new CacheEntry(key, value, new CacheEntryOptions());
            cache.Set(key, entry);
        }

        /// <summary>
        /// Create or update cache entry and set ttl on entry
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        /// <param name="ttl">Ttl for cache entry <see cref="CacheEntryOptions.Ttl"/></param>
        public static void Set(this ICache cache, string key, object value, TimeSpan ttl)
        {
            var entryOptions = new CacheEntryOptions()
            {
                Ttl = ttl,
            };
            
            var entry = new CacheEntry(key, value, entryOptions);
            
            cache.Set(key, entry);
        }

        /// <inheritdoc cref="Set(GH.DD.Cache.ICache,string,object,TimeSpan)"/>
        /// <param name="ttlSeconds">Ttl for cache entry in seconds/></param>
        public static void Set(this ICache cache, string key, object value, int ttlSeconds)
        {
            cache.Set(key, value, TimeSpan.FromSeconds(ttlSeconds));
        }
        
        /// <summary>
        /// Create or update cache entry and set ttl and callback on entry
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        /// <param name="ttl">Ttl for cache entry <see cref="CacheEntryOptions.Ttl"/></param>
        /// <param name="updateDataCallback">Callback for execute after <see cref="ttl"/> expired</param>
        public static void Set(this ICache cache, string key, object value, TimeSpan ttl,
            Func<object> updateDataCallback)
        {
            var entryOptions = new CacheEntryOptions()
            {
                Ttl = ttl,
                UpdateDataCallback = updateDataCallback,
                IsAutoDeleted = false
            };
            
            var entry = new CacheEntry(key, value, entryOptions);
            
            cache.Set(key, entry);
        }

        /// <inheritdoc cref="Set(GH.DD.Cache.ICache,string,object,TimeSpan)"/>
        /// <param name="ttlSeconds">Ttl for cache entry in seconds/></param>
        public static void Set(this ICache cache, string key, object value, int ttlSeconds,
            Func<object> updateDataCallback)
        {
            cache.Set(key, value, TimeSpan.FromSeconds(ttlSeconds), updateDataCallback);
        }

        /// <summary>
        /// Get Cache value if key exist and cast it to <see cref="TItem"/>. Or create cache entry
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Cache key</param>
        /// <param name="fabric">Fabric for build Cache value</param>
        /// <param name="options"><see cref="CacheEntryOptions"/> specified for <see cref="CacheEntry"/> if it not in Cache yet</param>
        /// <typeparam name="TItem">Type of Cache value</typeparam>
        /// <returns>Value of cache casted to <see cref="TItem"/></returns>
        public static TItem GetOrCreate<TItem>(this ICache cache, string key, Func<TItem> fabric, CacheEntryOptions options)
        {
            if (cache.TryGet<TItem>(key, out var value))
                return value;

            value = fabric();
            var cacheEntry = new CacheEntry(key, value, options);

            cache.Set(key, cacheEntry);

            return value;
        }
        
        /// <summary>
        /// Get Cache value if key exist and cast it to <see cref="TItem"/>. Or create cache entry
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Cache key</param>
        /// <param name="fabric">Fabric for build Cache value</param>
        /// <param name="ttl">Ttl for cache entry <see cref="CacheEntryOptions.Ttl"/></param>
        /// <typeparam name="TItem">Cast value to this Type</typeparam>
        /// <returns>Value of cache casted to <see cref="TItem"/></returns>
        public static TItem GetOrCreate<TItem>(this ICache cache, string key, Func<TItem> fabric, TimeSpan ttl)
        {
            var options = new CacheEntryOptions()
            {
                Ttl = ttl
            };
            
            return cache.GetOrCreate(key, fabric, options);
        }

        /// <inheritdoc cref="GetOrCreate{TItem}(GH.DD.Cache.ICache,string,System.Func{TItem},TimeSpan)"/>
        /// <param name="ttlSeconds">Ttl for cache entry in seconds/></param>
        public static TItem GetOrCreate<TItem>(this ICache cache, string key, Func<TItem> fabric, int ttlSeconds)
        {
            return cache.GetOrCreate(key, fabric, TimeSpan.FromSeconds(ttlSeconds));
        }
    }
}