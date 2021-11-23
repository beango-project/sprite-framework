using Microsoft.Extensions.DependencyInjection;
using Sprite.Modular;

namespace Sprite.Core.Tests.Modular
{
    public class TestModuleBase : SpriteModule
    {
        public bool ServiceConfigurationHasBenCalled { get; set; }

        public bool ConfigureHasBenCalled { get; set; }

        public override void ConfigureServices(IServiceCollection services)
        {
            ServiceConfigurationHasBenCalled = true;
        }

        public void Configure()
        {
            ConfigureHasBenCalled = true;
        }
    }
}