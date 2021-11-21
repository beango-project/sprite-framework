using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;

namespace Sprite.DependencyInjection.SimpleInjector
{
    public static class SpriteGenericHostBuilderForSimpleInjectorExtensions
    {
        public static IHostBuilder UseSpriteServiceProviderFactory(this IHostBuilder hostBuilder)
        {
            // var factory = CreateStashboxServiceProviderFactory(config => { config.WithSpriteInjection(); });
            var useSimpleInjector = hostBuilder.Build()
                .UseSimpleInjector(new Container());
            return
        }

        private static IServiceProviderFactory<TContainerBuilder> CreateSimpleInjectorServiceProviderFactory<TContainerBuilder>()
        {
            IServiceCollection services;
            services.AddSimpleInjector()
            var container = new Container();
            
        }
    }
}