using System;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection;

namespace Sprite.Modular
{
    public class OnApplicationInitContext : IServiceProviderAccessor
    {
        public IServiceProvider ServiceProvider { get; set; }

        public OnApplicationInitContext(IServiceProvider serviceProvider)
        {
            Guard.CheckNotNull(serviceProvider, nameof(serviceProvider));
            ServiceProvider = serviceProvider;
        }

        public T Get<T>()
        {
            return (T) ServiceProvider.GetRequiredService(typeof(T));
        }

        public object Get(Type type)
        {
            return ServiceProvider.GetRequiredService(type);
        }
    }
}