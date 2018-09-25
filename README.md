# Cache
Is simple cache for dotnet with feature async update element in background.

## Install
Install throught [nuget](https://www.nuget.org/packages/GH.DD.Cache/)

## Examples
### GetOrCreate
```c#
using GH.DD.Cache;

namespace MyApp
{
    public class CacheExample
    {
        private const string CacheKey = "SomeCacheKey";
        private const int CacheTtlSeconds = 3; 

        public CacheExample()
        {
            var cache = new MemoryCache();

            var value = cache.GetOrCreate(CacheKey, () => GetCacheValue("someArg"), CacheTtlSeconds);
            
            // Some actions
            Console.WriteLine(value);
        }
        
        private string GetCacheValue(string arg)
        {
            return "SomeValue arg:" + arg;
        }
    }
}
``` 

### Update cache entry throught callback
As first you need `Set` value with callback function.  
If Ttl on cache entry expired then next request to cache element will start callback update cache and will return old cache value.   
New cache value will available after callback finish

```c#
using GH.DD.Cache;

namespace MyApp
{
    public class CacheExample
    {
        private const string CacheKey = "SomeCacheKey";
        private const int CacheTtlSeconds = 3; 

        public CacheExample()
        {
            var cache = new MemoryCache();

            // You can warm you cache instead set "initial value" 
            // or wait for first TryGet request for start update callback
            cache.Set(CacheKey, "initial value", CacheTtlSeconds, UpdateCacheValue);

            if (!cache.TryGet(CacheKey, out var ValueNew))
            {
                // Some actions
            }
            
            // Some actions
            Console.WriteLine(ValueNew);
        }
        
        private string UpdateCacheValue()
        {
            return "Some new value";
        }
    }
}
``` 

## Notes
* TTL on cache element will check only if you `Get`/`TryGet` this element from cache
* By default during work update callback will return old cache value
* If during update cache element in callback will throw exception, when TTL on this element will not update. In this way if you try get element one more time it will start update callback again
