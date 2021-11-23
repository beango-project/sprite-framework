using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.ObjectMapping
{
    /// <summary>
    /// 对象映射空实现类
    /// </summary>
    [Component(ServiceLifetime.Singleton)]
    public class NullObjectMapper : IObjectMapper
    {
        public TDestination Map<TDestination>(object source)
        {
            throw new System.NotImplementedException();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            throw new System.NotImplementedException();
        }
    }
}