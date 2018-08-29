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

        private object _value;
        
        /// <inheritdoc cref="ICacheEntry.Key"/>
        public string Key { private set; get; }

        /// <inheritdoc cref="ICacheEntry.Value"/>
        public object Value
        {
            private set
            {
                lock (_locker)
                {
                    _value = value;
                }
            }
            get
            {
                lock (_locker)
                {
                    return _value;
                }
            }
        }

        /// <summary>
        /// Constructor for create new cache entry
        /// </summary>
        /// <param name="key">Key of cache</param>
        /// <param name="value">Value of cache</param>
        /// <param name="options"><see cref="CacheEntryOptions"/> of <see cref="CacheEntry"/></param>
        public CacheEntry(string key, object value, CacheEntryOptions options)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            _options = options;

            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (options.Ttl != null)
                _expireDateTime = DateTime.Now + options.Ttl;
        }
        
        /// <inheritdoc cref="ICacheEntry.IsAutoDeleted"/>
        public bool IsAutoDeleted => _options.IsAutoDeleted;

        /// <inheritdoc cref="ICacheEntry.IsExpired"/>
        public bool IsExpired()
        {
            // Prolong expire period on execution time of UpdateDataCallback
            if (_isRunningBeforeDeleteCallback)
                return false;
                
            if (!_expireDateTime.HasValue)
                return false;

            return DateTime.Now > _expireDateTime.Value;
        }

        /// <inheritdoc cref="ICacheEntry.ExecuteUpdateDataCallback"/>
        public async Task ExecuteUpdateDataCallback()
        {
            if (_options.UpdateDataCallback == null)
                return;

            _isRunningBeforeDeleteCallback = true;
            await Task.
                Run(_options.UpdateDataCallback)
                .ContinueWith(
                t => {
                    UpdateValue(t.Result);
                });
        }

        private void UpdateValue(object value)
        {
            if (Value.GetType() != value.GetType())
                throw new TypeAccessException(nameof(value));
            
            if (_options.Ttl != null)
                _expireDateTime = DateTime.Now + _options.Ttl;

            _isRunningBeforeDeleteCallback = false;
            Value = value;
        }
    }
}