using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Modular;

namespace Sprite.Data
{
    public class SpriteDataModule : SpriteModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbConnectionOptions>(services.GetConfiguration());
            services.TryAddTransient<IConnectionStringResolver,ConnectionStringResolver>();
        }
    }
}