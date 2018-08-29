namespace GH.DD.Cache
{
    /// <summary>
    /// Interface for Cache
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Get <see cref="ICacheEntry"/> from Cache by key. Will check expire Ttl timer and run callbacks Before/After delete if needed.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns><see cref="ICacheEntry"/> or Null if key not found</returns>
        ICacheEntry Get(string key);
        
        /// <summary>
        /// Set <see cref="ICacheEntry"/> in cache by key
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="entry">Cache entry <see cref="ICacheEntry"/></param>
        void Set(string key, ICacheEntry entry);
    }
}