using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Modular
{
    public interface IModule
    {
        void ConfigureServices(IServiceCollection services);
    }
}