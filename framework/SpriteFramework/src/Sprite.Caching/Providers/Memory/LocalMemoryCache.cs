using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Sprite.Caching.Providers.Memory
{
    public class LocalMemoryCache : ILocalMemoryCache
    {
        private readonly ConcurrentDictionary<string, CacheEntry> Cache;

        public LocalMemoryCache()
        {
            Cache = new ConcurrentDictionary<string, CacheEntry>();
        }

        public ValueTask<T?> TryGetAsync<T>(string cacheKey)
        {
            //If expired will remove this cache
            if (Cache.TryGetValue(cacheKey, out var entry))
            {
                if (!entry.IsExpired) //Check cache entry is expired
                {
                    var cacheEntry = entry as CacheEntry<T>;
                    cacheEntry.LastAccessed=DateTimeOffset.UtcNow;
                    return new ValueTask<T?>(cacheEntry.Value);
                }
             
                Cache.TryRemove(cacheKey, out _);
            }

            return new ValueTask<T?>((T?) default);
        }


        public ValueTask<CacheEntry<T>?> TryGetEntryAsync<T>(string cacheKey)
        {
            if (Cache.TryGetValue(cacheKey, out var cacheEntry))
            {
                if (!cacheEntry.IsExpired)
                {
                    cacheEntry.LastAccessed=DateTimeOffset.UtcNow;
                    return new ValueTask<CacheEntry<T>?>(cacheEntry as CacheEntry<T>);
                }

                Cache.TryRemove(cacheKey, out _);
            }

            return new ValueTask<CacheEntry<T>?>(default(CacheEntry<T>));
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, TimeSpan absoluteExpirationRelativeToNow)
        {
            var entry = new CacheEntry<T>(cacheKey,tValue, absoluteExpirationRelativeToNow);
            Cache[cacheKey] = entry;
            return new ValueTask<T>(tValue);
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, DateTimeOffset absoluteExpiration)
        {
            var entry = new CacheEntry<T>(cacheKey,tValue, absoluteExpiration);
            Cache[cacheKey] = entry;
            return new ValueTask<T>(tValue);
        }

        public ValueTask<bool> IsAvailableAsync(string cacheKey)
        {
            if (!cacheKey.IsNullOrWhiteSpace())
            {
                return new ValueTask<bool>(true);
            }

            throw new ArgumentException("");
        }

        public ValueTask CleanUpAsync()
        {
            foreach (var (key, value) in Cache)
            {
                if (value.IsExpired)
                {
                    Cache.TryRemove(key, out _);
                }
            }

            return new ValueTask();
        }

        public ValueTask RemoveAsync(string cacheKey)
        {
            Cache.TryRemove(cacheKey, out _);

            return new ValueTask();
        }
    }
}