using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Modular
{
    public interface IConfigureServicesProcessor : IModuleProcessor
    {
        void BeforeConfigureServices(IServiceCollection services);

        void AfterConfigureServices(IServiceCollection services);
    }
}