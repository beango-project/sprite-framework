using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Context;
using Sprite.Modular;

namespace Sprite.DependencyInjection
{
    public static class ServiceCollectionWithApplicationContextExtension
    {
        public static IMountSpriteApplicationContext AddSprite<TStartupModule>([NotNull] this IServiceCollection services,
            [CanBeNull] Action<SpriteApplicationCreateOptions> optionsAction = null)
            where TStartupModule : IModule
        {
            return SpriteApplication.Build<TStartupModule>(services, optionsAction);
        }
    }
}