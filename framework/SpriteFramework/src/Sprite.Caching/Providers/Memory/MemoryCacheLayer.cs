using System;
using System.Threading.Tasks;

namespace Sprite.Caching.Providers.Memory
{
    public class MemoryCacheLayer : ICacheLayer
    {
        public MemoryCacheLayer(ICacheProvider provider)
        {
            CacheProvider = provider;
        }


        private ICache _cache => CacheProvider.GetCache();

        public ICacheProvider CacheProvider { get; }

        public int HitStatistics { get; }

        public ValueTask<T?> TryGetAsync<T>(string cacheKey)
        {
            return _cache.TryGetAsync<T>(cacheKey);
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue)
        {
            throw new NotImplementedException();
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, TimeSpan absoluteExpirationRelativeToNow)
        {
            return _cache.SetAsync(cacheKey, tValue, absoluteExpirationRelativeToNow);
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, DateTimeOffset absoluteExpiration)
        {
            return _cache.SetAsync(cacheKey, tValue, absoluteExpiration);
        }

        public ValueTask CleanUpAsync()
        {
            return _cache.CleanUpAsync();
        }

        public ValueTask<bool> IsAvailableAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }
    }
}