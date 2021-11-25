using Microsoft.Extensions.DependencyInjection;
using Sprite.DynamicProxy;
using Sprite.Modular;
using Sprite.Tests;

namespace Sprite.DependencyInjection.DryIoc.Tests
{
    class SpriteModuleConfig : ModuleConfig
    {
        public override void Configure()
        {
            ImportModules();
        }
    }


    [Usage(typeof(SpriteModuleConfig))]
    public class SpriteDryIocTestModule : SpriteModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
        }
    }
}