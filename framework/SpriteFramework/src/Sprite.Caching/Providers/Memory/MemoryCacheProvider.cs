namespace Sprite.Caching.Providers.Memory
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly ICache _cache;

        public MemoryCacheProvider(ICache cache)
        {
            _cache = cache;
        }

        public ICache GetCache()
        {
            return _cache;
        }
    }
}