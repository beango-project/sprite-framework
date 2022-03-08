using Microsoft.Extensions.Hosting;

namespace Sprite.DependencyInjection.Grace
{
    public static class SpriteGenericHostBuilderForGraceExtensions
    {
        public static IHostBuilder UseSpriteServiceProviderFactory(this IHostBuilder hostBuilder)
        {
            var adapter = new GraceServiceProviderAdapter();
            adapter.Initialization();
            return hostBuilder.UseServiceProviderFactory(adapter.CreateServiceProviderFactory());
        }
    }
}