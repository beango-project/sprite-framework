using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.Tests;
using Xunit;

namespace Sprite.DependencyInjection.Grace.Tests.Context;

public class ApplicationContextIntegratedTests : GraceIntegratedTest
{
    [Fact]
    public void Conventional_App_Run_Test()
    {
        var applicationContext = SpriteApplication.Build<SpriteGraceTestModule>(options => { options.UseGrace(); });
        applicationContext.RootModuleType.ShouldBe(typeof(SpriteGraceTestModule));
        var rootStartupModule = applicationContext.Services.GetRequestSingletonInstance<SpriteGraceTestModule>();

        applicationContext.Run();

        var rootStartupModuleWithRunningGet = applicationContext.ServiceProvider.GetRequiredService<SpriteGraceTestModule>();

        rootStartupModuleWithRunningGet.ShouldBeSameAs(rootStartupModule);
        applicationContext.Shutdown();
    }

    [Fact]
    public void Mount_App_Run_Test()
    {
        var applicationContext = ApplicationContext;

        applicationContext.RootModuleType.ShouldBe(typeof(SpriteGraceTestModule));
        var rootStartupModule = applicationContext.Services.GetRequestSingletonInstance<SpriteGraceTestModule>();

        var rootStartupModuleWithRunningGet = applicationContext.ServiceProvider.GetRequiredService<SpriteGraceTestModule>();

        rootStartupModuleWithRunningGet.ShouldBeSameAs(rootStartupModule);
        applicationContext.Shutdown();
    }
}