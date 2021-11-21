using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Caching;
using Sprite.Caching.Providers.Memory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionCachingExtension
    {
        public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheOptions> action)
        {
            // var options = new CacheOptions();
            services.Configure(action);
            services.TryAddSingleton<ICacheManager, CacheManager>();
            return services;
        }

        public static void AddMemoryCache(this CacheOptions cacheOptions)
        {
            cacheOptions.AddCacheLayer(new MemoryCacheLayer(new MemoryCacheProvider(new LocalMemoryCache())));
            cacheOptions.AddWorks(new AutoCleanupBackgroundWork
            {
                ExecutionFrequency = cacheOptions.CleanupFrequency
            });
        }
    }
}