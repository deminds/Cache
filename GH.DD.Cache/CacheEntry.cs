using System;
using System.Threading.Tasks;

namespace GH.DD.Cache
{
    /// <summary>
    /// Cache entry placed in cache object by key
    /// </summary>
    public class CacheEntry : ICacheEntry
    {
        private DateTime? _expireDateTime;
        private readonly CacheEntryOptions _options;
        
        private readonly object _locker = new object();
        private bool _isRunningBeforeDeleteCallback;
        
        /// <inheritdoc cref="ICacheEntry.Key"/>
        public object Key { private set; get; }
        
        /// <inheritdoc cref="ICacheEntry.Value"/>
        public object Value { private set; get; }
        
        /// <inheritdoc cref="ICacheEntry.IsAutoDeleted"/>
        public bool IsAutoDeleted => _options.IsAutoDeleted;

        /// <inheritdoc cref="ICacheEntry.IsExpired"/>
        public bool IsExpired()
        {
            // Prolong expire period on execution time of BeforeDeleteCallback
            if (!_options.IsAutoDeleted && _isRunningBeforeDeleteCallback)
                return false;
                
            if (!_expireDateTime.HasValue)
                return false;

            return DateTime.Now > _expireDateTime.Value;
        }

        /// <summary>
        /// Constructor for create new cache entry
        /// </summary>
        /// <param name="key">Key of cache</param>
        /// <param name="value">Value of cache</param>
        /// <param name="options"><see cref="CacheEntryOptions"/> of <see cref="CacheEntry"/></param>
        public CacheEntry(object key, object value, CacheEntryOptions options)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            _options = options;

            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (options.Ttl != null)
                _expireDateTime = DateTime.Now + options.Ttl;
        }

        /// <inheritdoc cref="ICacheEntry.UpdateValue"/>
        public void UpdateValue(object value)
        {
            if (Value.GetType() != value.GetType())
                throw new ArgumentNullException(nameof(value));
            
            if (_options.Ttl != null)
                _expireDateTime = DateTime.Now + _options.Ttl;

            lock (_locker)
            {
                Value = value;
            }
        }

        /// <inheritdoc cref="ICacheEntry.ExecuteBeforeDeleteCallback"/>
        public void ExecuteBeforeDeleteCallback()
        {
            if (_options.BeforeDeleteCallback == null)
                return;

            _isRunningBeforeDeleteCallback = true;
            var task = new Task(_options.BeforeDeleteCallback(this));
            task.Start();
            task.ContinueWith((t) => _isRunningBeforeDeleteCallback = false);
        }
        
        /// <inheritdoc cref="ICacheEntry.ExecuteAfterDeleteCallback"/>
        public void ExecuteAfterDeleteCallback()
        {
            if (_options.AfterDeleteCallback == null) 
                return;
            
            new Task(_options.AfterDeleteCallback(this)).Start();
        }
    }
}