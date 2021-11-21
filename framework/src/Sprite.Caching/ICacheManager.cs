using System;
using System.Threading.Tasks;

namespace Sprite.Caching
{
    public interface ICacheManager
    {
        T Provider<T>() where T : ICache;

        public ValueTask<T?> TryGetAsync<T>(string cacheKey);

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, TimeSpan absoluteExpirationRelativeToNow);


        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, DateTimeOffset absoluteExpiration);


        // ValueTask<T> GetOrSetAsync<T>(string cacheKey, Func<T, Task<T>> getter, CacheSettings settings);

        public ValueTask<CacheEntry<T>?> TryGetEntryAsync<T>(string cacheKey);
    }
}