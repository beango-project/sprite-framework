using System;
using Microsoft.Extensions.Hosting;
using Stashbox;
using Stashbox.Configuration;
using Stashbox.Extensions.Dependencyinjection;

// ReSharper disable IdentifierTypo

namespace Sprite.DependencyInjection.Stashbox
{
    public static class SpriteGenericHostBuilderForStashboxExtensions
    {
        public static IHostBuilder UseSpriteServiceProviderFactory(this IHostBuilder hostBuilder)
        {
            var factory = CreateStashboxServiceProviderFactory(config => { config.WithSpriteInjection(); });
            return hostBuilder.UseServiceProviderFactory(factory);
        }

        private static StashboxServiceProviderFactory CreateStashboxServiceProviderFactory(Action<ContainerConfigurator> containerConfig)
        {
            
            var container = new StashboxContainer(containerConfig);
            container.Configure(config =>
            {
                // config.WithLifetimeValidation()
                // config.WithAutoMemberInjection(SpriteStashboxConfiguration.AutoMemberInjectionRule, f =>
                // {
                //     var attr =  f.GetAttributeWithDefined<AutowiredAttribute>();
                //     if (attr != null)
                //     {
                //         // container
                //         return true;
                //     }
                //
                //     return false;
                // });
            });
            return new StashboxServiceProviderFactory(container);
        }
    }
}