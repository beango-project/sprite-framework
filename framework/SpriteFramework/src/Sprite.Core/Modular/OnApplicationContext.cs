using System;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection;

namespace Sprite.Modular
{
    public class OnApplicationContext : IServiceProviderAccessor
    {
        public OnApplicationContext(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; set; }

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