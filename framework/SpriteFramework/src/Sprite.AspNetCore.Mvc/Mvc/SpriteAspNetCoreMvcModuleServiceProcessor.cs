using Microsoft.Extensions.DependencyInjection;
using Sprite.Modular;

namespace Sprite.AspNetCore.Mvc
{
    public class SpriteAspNetCoreMvcModuleProcessor : IConfigureServicesProcessor
    {
        public int Order => 0;

        public void BeforeConfigureServices(IServiceCollection services)
        {
            services.AddRegistrationRules(new SpriteAspNetCoreMvcRegistrationRules());
        }

        public void AfterConfigureServices(IServiceCollection services)
        {
        }
    }
}