using JetBrains.Annotations;

namespace Sprite.Caching
{
    public interface ICacheLayerListener
    {
        int HitStatistics { get; }


        [NotNull]
        CacheHeatHolder CacheHeatHolder { get; }
    }
}