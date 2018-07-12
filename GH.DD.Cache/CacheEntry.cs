using System;
using System.Threading.Tasks;

namespace GH.DD.Cache
{
    public class CacheEntry : ICacheEntry
    {
        private readonly DateTime? _expireDateTime = null;
        private readonly CacheEntryOptions _options;
        
        public string Key { private set; get;  }
        public object Value { private set; get; }
        
        public bool IsAutoDeleted => _options.IsAutoDeleted;

        public bool IsExpired()
        {
            if (!_expireDateTime.HasValue)
                return false;

            return DateTime.Now > _expireDateTime.Value;
        }

        public CacheEntry(string key, object value, CacheEntryOptions options)
        {
            Key = string.IsNullOrWhiteSpace(key) 
                    ? throw new ArgumentNullException(nameof(key)) 
                    : key;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            _options = options;

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.Ttl != null)
                _expireDateTime = DateTime.Now + options.Ttl;
        }

        public void ExecuteBeforeDeleteCallback()
        {
            if (_options.BeforeDeleteCallback == null) return;
            
            new Task(_options.BeforeDeleteCallback(Key, Value)).Start();
        }
        
        public void ExecuteAfterDeleteCallback()
        {
            if (_options.AfterDeleteCallback == null) return;
            
            new Task(_options.AfterDeleteCallback(Key, Value)).Start();
        }
    }
}