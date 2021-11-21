using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Sprite.Caching
{
    public class CacheManager : ICacheManager
    {
        // private readonly IFlushableCacheStack _stack;

        private readonly ICacheStack _stack;

        private readonly CacheOptions _options;


        public CacheManager(IOptionsMonitor<CacheOptions> options)
        {
            _options = options.CurrentValue;
            Caches = new List<ICache>();

            var works = new List<IBackgroundWork>();

            foreach (var work in _options.Works)
            {
                works.AddIfNotContains(work);
            }

            _stack = new CacheStack(_options.CacheLayers.ToArray(), works.ToArray());
        }

        public IList<ICache> Caches { get; }

        public T Provider<T>() where T : ICache
        {
            return (T) Caches.FirstOrDefault(x => x.GetType() == typeof(T));
        }


        // public ValueTask FlushAsync()
        // {
        //     return _stack.FlushAsync();
        // }
        //
        // public ValueTask CleanupAsync()
        // {
        //     return _stack.CleanupAsync();
        // }
        //
        // public ValueTask RemoveAsync(string cacheKey)
        // {
        //     return _stack.EvictAsync(cacheKey);
        // }

        public ValueTask<T?> TryGetAsync<T>(string cacheKey)
        {
            return _stack.TryGetAsync<T>(cacheKey);
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, TimeSpan absoluteExpirationRelativeToNow)
        {
            return _stack.SetAsync(cacheKey, tValue, absoluteExpirationRelativeToNow);
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, DateTimeOffset absoluteExpiration)
        {
            return _stack.SetAsync(cacheKey, tValue, absoluteExpiration);
        }

        public ValueTask<CacheEntry<T>?> TryGetEntryAsync<T>(string cacheKey)
        {
            throw new NotImplementedException();
        }
    }
}