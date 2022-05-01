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
            var adapter = new DryIocServiceProviderAdapter();
            adapter.Initialization();
            var factory = adapter.CreateServiceProviderFactory();
            services.TryAddSingleton((IServiceProviderFactory<IContainer>)factory);
            return factory;
        }
    }
}