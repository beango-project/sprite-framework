using Microsoft.Extensions.DependencyInjection;
using Sprite.Data.DependencyInjection;
using Sprite.Modular;

namespace Sprite.Data
{
    public class SpriteAspNetCoreMvcModuleServiceProcessor: IConfigureServicesProcessor
    {
        public int Order => 0;
        public void BeforeConfigureServices(IServiceCollection services)
        {
            services.AddRegistrationRules(new SpriteRepositoryRegistrationRules());
        }

        public void AfterConfigureServices(IServiceCollection services)
        {
            
        }
    }
}