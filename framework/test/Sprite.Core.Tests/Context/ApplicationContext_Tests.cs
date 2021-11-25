using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.Context;
using Sprite.Core.Tests.Modular;
using Xunit;

namespace Sprite.Core.Tests.Context
{
    public class ApplicationContext_Tests
    {
        [Fact]
        public void Conventional_App_Run_Test()
        {
            var applicationContext = new ConventionalSpriteApplicationContext(typeof(RootStartupModule));

            applicationContext.RootModuleType.ShouldBe(typeof(RootStartupModule));
            applicationContext.Run();
            var rootStartupModule = applicationContext.Services.GetRequestSingletonInstance<RootStartupModule>();
            var module = applicationContext.Services.GetRequestSingletonInstance<EmptyModule>();

            module.ServiceConfigurationHasBenCalled.ShouldBeTrue();
            module.ConfigureHasBenCalled.ShouldBeTrue();

            var rootStartupModuleWithRunningGet = applicationContext.ServiceProvider.GetRequiredService<RootStartupModule>();
            rootStartupModuleWithRunningGet.ConfigureInjectModule.ShouldBe(rootStartupModuleWithRunningGet);
            rootStartupModuleWithRunningGet.ShouldBeSameAs(rootStartupModule);
            applicationContext.Shutdown();
            
            //Shutdown 
            rootStartupModuleWithRunningGet.ShutdownHasCelled.ShouldBeTrue();
        }
        
        [Fact]
        public void Mount_App_Run_Test()
        {
            IServiceCollection services = new ServiceCollection();
            var applicationContext = new MountSpriteApplicationContext(typeof(RootStartupModule), services, options =>
            {
                options.Services.ShouldBe(services);
            });

            applicationContext.RootModuleType.ShouldBe(typeof(RootStartupModule));
            var serviceProvider = services.BuildServiceProvider();
            applicationContext.Run(serviceProvider);
            
            var rootStartupModule = applicationContext.Services.GetRequestSingletonInstance<RootStartupModule>();
            var module = applicationContext.Services.GetRequestSingletonInstance<EmptyModule>();
            
            module.ServiceConfigurationHasBenCalled.ShouldBeTrue();
            module.ConfigureHasBenCalled.ShouldBeTrue();
            
            var rootStartupModuleWithRunningGet = applicationContext.ServiceProvider.GetRequiredService<RootStartupModule>();
            rootStartupModuleWithRunningGet.ConfigureInjectModule.ShouldBe(rootStartupModuleWithRunningGet);
            rootStartupModuleWithRunningGet.ShouldBeSameAs(rootStartupModule);
            applicationContext.Shutdown();
            
            //Shutdown 
            rootStartupModuleWithRunningGet.ShutdownHasCelled.ShouldBeTrue();
        }
    }
}