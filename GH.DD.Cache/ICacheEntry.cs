namespace GH.DD.Cache
{
    public interface ICacheEntry
    {
        /// <summary>
        /// Key of cache
        /// </summary>
        object Key { get; }
        
        /// <summary>
        /// Value of cache
        /// </summary>
        object Value { get; }
        
        /// <summary>
        /// AutoDeleted flag. If is true then after Ttl expire cache entry will be remove and run callbacks
        /// If is false then only callbacks will run after Ttl expire
        /// Can be set in <see cref="CacheEntryOptions"/>
        /// </summary>
        bool IsAutoDeleted { get; }
        
        /// <summary>
        /// Check Ttl is expired or not
        /// </summary>
        /// <returns></returns>
        bool IsExpired();

        /// <summary>
        /// Set new value in <see cref="CacheEntry"/>
        /// </summary>
        /// <param name="value">New value</param>
        void UpdateValue(object value);

        /// <summary>
        /// Execute <see cref="CacheEntryOptions.BeforeDeleteCallback"/> in separate Task
        /// </summary>
        void ExecuteBeforeDeleteCallback();
        
        /// <summary>
        /// Execute <see cref="CacheEntryOptions.AfterDeleteCallback"/> in separate Task
        /// </summary>
        void ExecuteAfterDeleteCallback();
    }
}