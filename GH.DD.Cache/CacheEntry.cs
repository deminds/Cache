using System;
using System.Threading.Tasks;

namespace GH.DD.Cache
{
    public class CacheEntry : ICacheEntry
    {
        private DateTime? _expireDateTime = null;
        private readonly CacheEntryOptions _options;
        
        private readonly object _locker = new object();
        private bool _isRunningBeforeDeleteCallback;
        
        public object Key { private set; get; }
        public object Value { private set; get; }
        
        public bool IsAutoDeleted => _options.IsAutoDeleted;

        public bool IsExpired()
        {
            if (!_options.IsAutoDeleted && _isRunningBeforeDeleteCallback)
                return false;
                
            if (!_expireDateTime.HasValue)
                return false;

            return DateTime.Now > _expireDateTime.Value;
        }

        public CacheEntry(object key, object value, CacheEntryOptions options)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            _options = options;

            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (options.Ttl != null)
                _expireDateTime = DateTime.Now + options.Ttl;
        }

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

        public void ExecuteBeforeDeleteCallback()
        {
            if (_options.BeforeDeleteCallback == null)
                return;

            _isRunningBeforeDeleteCallback = true;
            var task = new Task(_options.BeforeDeleteCallback(this));
            task.Start();
            task.ContinueWith((t) => _isRunningBeforeDeleteCallback = false);
        }
        
        public void ExecuteAfterDeleteCallback()
        {
            if (_options.AfterDeleteCallback == null) 
                return;
            
            new Task(_options.AfterDeleteCallback(this)).Start();
        }
    }
}