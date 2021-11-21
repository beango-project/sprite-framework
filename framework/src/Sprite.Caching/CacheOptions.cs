using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Sprite.Caching
{
    public class CacheOptions
    {
        private readonly IList<ICacheLayer> _cacheLayers = new List<ICacheLayer>();
        private readonly IList<IBackgroundWork> _works = new List<IBackgroundWork>();

        public TimeSpan CleanupFrequency { get; set; }

        public IReadOnlyList<ICacheLayer> CacheLayers => _cacheLayers.ToImmutableList();
        public IReadOnlyList<IBackgroundWork> Works => _works.ToImmutableList();


        public void AddCacheLayer(params ICacheLayer[] cacheLayers)
        {
            foreach (var cacheLayer in cacheLayers)
            {
                _cacheLayers.AddIfNotContains(cacheLayer);
            }
        }

        public void AddWorks(params IBackgroundWork[] works)
        {
            foreach (var work in works)
            {
                _works.AddIfNotContains(work);
            }
        }
    }
}