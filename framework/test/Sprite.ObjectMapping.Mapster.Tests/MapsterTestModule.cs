using System;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Modular;

namespace Sprite.ObjectMapping.Mapster.Tests
{
    class MapsterTestConfig : ModuleConfig
    {
        public override Type[] ImportModules()
        {
            return new[] { typeof(SpriteMapsterModule) };
        }
    }

    [Usage(typeof(MapsterTestConfig))]
    public class MapsterTestModule : SpriteModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MapsterOptions>(options => { options.AddMaps<MapsterTestModule>(); });
        }
    }
}