using System;
using System.Threading.Tasks;

namespace Sprite.Caching
{
    public interface ICacheLayer
    {
        ICacheProvider CacheProvider { get; }

        int HitStatistics { get; }

        public ValueTask<T?> TryGetAsync<T>(string cacheKey);


        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue);


        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, TimeSpan absoluteExpirationRelativeToNow);


        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, DateTimeOffset absoluteExpiration);


        // ValueTask<T> GetOrSetAsync<T>(string cacheKey, Func<T, Task<T>> getter, CacheSettings settings);

        ValueTask CleanUpAsync();

        ValueTask<bool> IsAvailableAsync(string cacheKey);
    }
}