using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Context
{
    public class MountSpriteApplicationContext : SpriteApplicationContextBase, IMountSpriteApplicationContext
    {
        public MountSpriteApplicationContext([NotNull] Type rootModuleType, [NotNull] IServiceCollection services, [CanBeNull] Action<SpriteApplicationCreateOptions>
            optionAction) : base(rootModuleType, services, optionAction)
        {
            services.AddSingleton<IMountSpriteApplicationContext>(this);
        }

        public void Run(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            SetServiceProvider(serviceProvider);
            InitializeModules();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (ServiceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }
        }
    }
}