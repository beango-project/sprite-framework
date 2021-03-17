using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Modular
{
    public interface IModule
    {
        void ConfigureServices(IServiceCollection service);

        void Configure(OnApplicationInitContext context);
    }
}