namespace Sprite.Caching
{
    public interface ISupportBindCacheStack
    {
        ICacheStack CacheStack { get; }

        void BindCacheStack(ICacheStack cacheStack);
    }
}