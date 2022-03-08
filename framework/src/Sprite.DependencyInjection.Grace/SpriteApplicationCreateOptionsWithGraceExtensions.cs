using Grace.DependencyInjection;
using Grace.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sprite.DependencyInjection.Grace
{
    public static class SpriteApplicationCreateOptionsWithGraceExtensions
    {
        public static void UseGrace(this SpriteApplicationCreateOptions options)
        {
            options.Services.AddGraceServiceProviderFactory();
        }

        public static GraceServiceProviderFactory AddGraceServiceProviderFactory(this IServiceCollection services)
        {
            var adapter = new GraceServiceProviderAdapter();
            adapter.Initialization();
            var factory = adapter.CreateServiceProviderFactory();
            services.TryAddSingleton((IServiceProviderFactory<IInjectionScope>)factory);
            return factory;
        }
    }
}