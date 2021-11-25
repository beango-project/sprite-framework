using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.Context;
using Xunit;

namespace Sprite.DependencyInjection.DryIoc.Tests
{
    public class ApplicationContextIntegratedTests : DryIocIntegratedTest
    {
        [Fact]
        public void Conventional_App_Run_Test()
        {
            var applicationContext = SpriteApplication.Build<SpriteDryIocTestModule>(options => { options.UseDryIoc(); });
            applicationContext.RootModuleType.ShouldBe(typeof(SpriteDryIocTestModule));
            var rootStartupModule = applicationContext.Services.GetRequestSingletonInstance<SpriteDryIocTestModule>();

            applicationContext.Run();

            var rootStartupModuleWithRunningGet = applicationContext.ServiceProvider.GetRequiredService<SpriteDryIocTestModule>();

            rootStartupModuleWithRunningGet.ShouldBeSameAs(rootStartupModule);
            applicationContext.Shutdown();
        }

        [Fact]
        public void Mount_App_Run_Test()
        {
            var applicationContext = ApplicationContext;

            applicationContext.RootModuleType.ShouldBe(typeof(SpriteDryIocTestModule));
            var rootStartupModule = applicationContext.Services.GetRequestSingletonInstance<SpriteDryIocTestModule>();

            var rootStartupModuleWithRunningGet = applicationContext.ServiceProvider.GetRequiredService<SpriteDryIocTestModule>();

            rootStartupModuleWithRunningGet.ShouldBeSameAs(rootStartupModule);
            applicationContext.Shutdown();
        }
    }
}