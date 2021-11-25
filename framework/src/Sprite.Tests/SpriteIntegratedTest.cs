using System;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Context;
using Sprite.DependencyInjection;
using Sprite.Modular;

namespace Sprite.Tests
{
    public abstract class SpriteIntegratedTest<TStartupModule> : SpriteTestsServiceProvider, IDisposable
        where TStartupModule : ISpriteModule
    {
        protected ISpriteApplicationContext ApplicationContext { get; }

        protected override IServiceProvider ServiceProvider => ApplicationContext.ServiceProvider;

        protected IServiceProvider RootOrOuterServiceProvider { get; }

        private IServiceScope _scope;

        protected SpriteIntegratedTest()
        {
            var services = new ServiceCollection();
            var app = services.AddSprite<TStartupModule>(SetAppCreateOptions);
            ApplicationContext = app;
            RootOrOuterServiceProvider = BuildServiceProvider(services);
            _scope = RootOrOuterServiceProvider.CreateScope();
            app.Run(RootOrOuterServiceProvider);
        }

        protected virtual void SetAppCreateOptions(SpriteApplicationCreateOptions options)
        {
        }

        protected virtual IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProviderFromFactory();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}