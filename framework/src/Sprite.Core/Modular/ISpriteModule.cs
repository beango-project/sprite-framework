using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Modular
{
    public interface ISpriteModule
    {
        void ConfigureServices(IServiceCollection services);
    }
}