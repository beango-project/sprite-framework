using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Context
{
    public class ConventionalSpriteApplicationContext : SpriteApplicationContextBase, IConventionalSpriteApplicationContext
    {
        public ConventionalSpriteApplicationContext([NotNull] Type rootModuleType, [CanBeNull] Action<SpriteApplicationCreateOptions> optionAction)
            : this(rootModuleType, new ServiceCollection(), optionAction)
        {
        }

        private ConventionalSpriteApplicationContext([NotNull] Type rootModuleType, [NotNull] IServiceCollection services,
            [CanBeNull] Action<SpriteApplicationCreateOptions> optionAction) : base(rootModuleType, services, optionAction)
        {
            Services.AddSingleton<IConventionalSpriteApplicationContext>(this);
        }

        public IServiceScope ServiceScope { get; private set; }


        public void Run()
        {
            ServiceScope = Services.BuildServiceProviderFromFactory().CreateScope();
            SetServiceProvider(ServiceScope.ServiceProvider);
            Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
            ServiceScope.Dispose();
        }
    }
}