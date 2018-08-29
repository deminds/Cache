using System;

namespace GH.DD.Cache
{
    public class CacheEntryOptions
    {
        /// <summary>
        /// Ttl for auto delete cache entry and run callbacks
        /// Default is Null. Cache Entry will never be remove
        /// </summary>
        public TimeSpan? Ttl { set; get; } = null;
        
        /// <summary>
        /// AutoDeleted flag. If is true then after <see cref="Ttl"/> expire cache entry will be remove and run callbacks
        /// If is false then only callbacks will run after <see cref="Ttl"/> expire
        /// Default is True. Cache Entry will remove after <see cref="Ttl"/> expire
        /// </summary>
        public bool IsAutoDeleted { set; get; } = true;
        
        /// <summary>
        /// Callback <see cref="BeforeDeleteDelegate"/> will be run after <see cref="Ttl"/> expire and before remove cache entry
        /// Default is Null
        /// </summary>
        public Action<ICacheEntry> BeforeDeleteCallback { set; get; } = null;
        
        /// <summary>
        /// Callback <see cref="AfterDeleteDelegate"/> will be run after <see cref="Ttl"/> expire and after remove cache entry.
        /// If <see cref="IsAutoDeleted"/> is false then cache entry will not be remove.
        /// Default is Null
        /// </summary>
        public Action<ICacheEntry> AfterDeleteCallback { set; get; } = null;
    }
}