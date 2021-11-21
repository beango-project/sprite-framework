using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sprite.Caching.BackgroundWorks;

namespace Sprite.Caching
{
    public class CacheStack : ICacheStack
    {
        private IBackgroundWorkManager _backgroundWorks;
        private readonly IList<ICacheLayer> _cacheLayers;


        public CacheStack(ICacheLayer[] cacheLayers, IBackgroundWork[] works)
        {
            Check.NotNull(cacheLayers, nameof(cacheLayers));
            _cacheLayers = cacheLayers;
            // _backgroundWorks = new Works(works);
            // _backgroundWorks.BindingCacheStack(this);
            // Task.Run(async () => { await _backgroundWorks.Run(); }).ConfigureAwait(false);
        }


        public ValueTask<T?> TryGetAsync<T>(string cacheKey)
        {
            foreach (var cacheLayer in _cacheLayers)
            {
                var value = cacheLayer.TryGetAsync<T>(cacheKey);
                if (value != default)
                {
                    return value;
                }
            }

            return default;
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, TimeSpan absoluteExpirationRelativeToNow)
        {
            foreach (var cacheLayer in _cacheLayers)
            {
                var value = cacheLayer.SetAsync(cacheKey, tValue, absoluteExpirationRelativeToNow);
                if (value != default)
                {
                    return value;
                }
            }

            return new ValueTask<T>(default(T));
        }

        public ValueTask<T> SetAsync<T>(string cacheKey, T tValue, DateTimeOffset absoluteExpiration)
        {
            throw new NotImplementedException();
        }

        public ValueTask<CacheEntry<T>?> TryGetEntryAsync<T>(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> IsAvailableAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanUpAsync()
        {
            foreach (var layer in _cacheLayers)
            {
                layer.CleanUpAsync();
            }

            return new ValueTask();
        }

        public ValueTask RemoveAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }
    }
}