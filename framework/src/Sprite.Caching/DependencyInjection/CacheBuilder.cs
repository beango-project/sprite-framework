using System.Collections.Generic;
using Sprite.Caching;

namespace Microsoft.Extensions.DependencyInjection
{
    public class CacheBuilder
    {
        private IList<ICacheProvider> _providers;

        public CacheBuilder()
        {
            _providers = new List<ICacheProvider>();
        }

        // public CacheBuilder AddMemoryCache()
        // {
        //     _providers.Add(new MemoryCacheProvider());
        //     return this;
        // }
    }
}