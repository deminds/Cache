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
        /// Callback <see cref="UpdateDataCallback"/> will be run after <see cref="Ttl"/> expire and before remove cache entry
        /// Default is Null
        /// </summary>
        public Func<object> UpdateDataCallback { set; get; } = null;
    }
}