using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sprite.DependencyInjection.DryIoc
{
    public static class SpriteApplicationCreateOptionsWithDryIocExtensions
    {
        public static void UseDryIoc(this SpriteApplicationCreateOptions options)
        {
            options.Services.AddDryIocServiceProviderFactory();
        }

        public static DryIocServiceProviderFactory AddDryIocServiceProviderFactory(this IServiceCollection services)
        {
            var container = DryIocServiceProviderBuilder.Build();
            var factory = new DryIocServiceProviderFactory(container);
            services.TryAddSingleton((IServiceProviderFactory<IContainer>)factory);
            return factory;
        }
    }
}