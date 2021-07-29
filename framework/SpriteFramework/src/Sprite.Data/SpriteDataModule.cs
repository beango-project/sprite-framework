using Microsoft.Extensions.DependencyInjection;
using Sprite.Modular;

namespace Sprite.Data
{
    public class SpriteDataModule : Module
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // services.Configure<DbConnectionOptions>(services.GetConfiguration());
        }
    }
}