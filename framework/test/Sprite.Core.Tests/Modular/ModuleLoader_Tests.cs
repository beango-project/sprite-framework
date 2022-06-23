using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.DependencyInjection;
using Sprite.Modular;
using Xunit;
using NotImplementedException = System.NotImplementedException;

namespace Sprite.Core.Tests.Modular
{
    public class ModuleLoader_Tests
    {
        [Fact]
        public void ModuleConfigure_Depend_Modules_ShouldBe_Import_Modules()
        {
            var configure = new RootStartupModuleConfig();
            configure.Configure();
            configure.ImportModules().Length.ShouldBe(1);
            configure.ImportModules()[0].ShouldBe(typeof(EmptyModule));
        }

        [Fact]
        public void ModuleLoader_Test()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new SwapSpace());

            var moduleLoader = new ModuleLoader(services, typeof(RootStartupModule));
            var moduleDefinitions = moduleLoader.LoadModules();

            moduleDefinitions.Count.ShouldBe(2);
            moduleDefinitions[0].Module.ShouldBe(typeof(EmptyModule));
            moduleDefinitions[1].Module.ShouldBe(typeof(RootStartupModule));

            var firstServiceWithBefore = services.FirstOrDefault(x => x.ServiceType == typeof(FirstServiceWithBefore));
            var firstServiceWithAfter = services.FirstOrDefault(x => x.ServiceType == typeof(FirstServiceWithAfter));
            var secondServiceWithBefore = services.FirstOrDefault(x => x.ServiceType == typeof(SecondServiceWithBefore));
            var secondServiceWithAfter = services.FirstOrDefault(x => x.ServiceType == typeof(SecondServiceWithAfter));


            //processor

            Assert.True(services.IndexOf(firstServiceWithBefore) < services.IndexOf(firstServiceWithAfter));
            Assert.True(services.IndexOf(secondServiceWithBefore) < services.IndexOf(secondServiceWithAfter));
            Assert.True(services.IndexOf(firstServiceWithBefore) < services.IndexOf(secondServiceWithBefore));
            var processors = moduleDefinitions[1].Processors;
            processors.Count.ShouldBe(3);
        }
    }

    public class RootStartupModuleConfig : ModuleConfig
    {
        public override Type[] ImportModules()
        {
            return new[] { typeof(EmptyModule) };
        }

        public override void Configure()
        {
            SkipAutoScanRegister = true;
        }
    }

    [Usage(typeof(RootStartupModuleConfig))]
    public class RootStartupModule : SpriteModule
    {
        public SpriteModule ConfigureInjectModule { get; set; }

        public bool ShutdownHasCelled { get; set; }

        public override void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(RootStartupModule rootStartupModule)
        {
            ConfigureInjectModule = rootStartupModule;
        }
    }

    public class FirstServiceWithBefore
    {
    }

    public class FirstServiceWithAfter
    {
    }

    public class SecondServiceWithBefore
    {
    }

    public class SecondServiceWithAfter
    {
    }

    public class FirstConfigureServicesProcessor : IConfigureServicesProcessor
    {
        public int Order => 0;

        public void BeforeConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new FirstServiceWithBefore());
        }

        public void AfterConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new FirstServiceWithAfter());
        }
    }

    public class SecondConfigureServicesProcessor : IConfigureServicesProcessor
    {
        public int Order => 1;

        public void BeforeConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new SecondServiceWithBefore());
        }

        public void AfterConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new SecondServiceWithAfter());
        }
    }

    public class RootModuleShutdownProcessor : IModuleShutdownProcessor
    {
        public int Order { get; }

        public void Shutdown(OnApplicationContext context)
        {
            context.ServiceProvider.GetRequiredService<RootStartupModule>().ShutdownHasCelled = true;
        }
    }
}