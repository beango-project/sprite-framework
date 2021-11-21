using Sprite.Caching;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionRedisCachingExtension
    {
        public static void AddRedisCache(this CacheOptions cacheOptions, ConnectionMultiplexer connection, int databaseIndex = -1)
        {
            // cacheOptions.AddProviders(new RedisCacheProvider(new RedisCache(connection, databaseIndex)));
            // var cacheProviders = cacheOptions.CacheProviders;
            // var caches = new List<ICacheLayer>();
            // foreach (var cache in cacheProviders)
            // {
            //     if (cache is RedisCache redisCache)
            //     {
            //         caches.Add(redisCache);
            //     }
            // }

            // cacheOptions.AddExtensions(new RedisLockExtension(connection),
            //     new RedisRemoteEvictionExtension(connection, caches.ToArray()));
        }
    }
}