using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Context;
using Sprite.Modular;

namespace Sprite
{
    public class SpriteApplication
    {
        // public IHost HostInstance { get; private set; }
        //
        // public IServiceCollection services { get; set; }
        //
        // public static void Run(Type rootModuleType, Action<IHostBuilder> hostBuilderOptions)
        // {
        //  
        // }

        private SpriteApplication()
        {
        }


        public static IConventionalSpriteApplicationContext Build<TRootModuleType>([CanBeNull] Action<SpriteApplicationCreateOptions> optionAction = null)
            where TRootModuleType : IModule
        {
            return new ConventionalSpriteApplicationContext(typeof(TRootModuleType), optionAction);
        }

        public static IMountSpriteApplicationContext Build<TRootModuleType>([NotNull] IServiceCollection services,
            [CanBeNull] Action<SpriteApplicationCreateOptions> optionAction = null)
            where TRootModuleType : IModule
        {
            return new MountSpriteApplicationContext(typeof(TRootModuleType), services, optionAction);
        }
        //
        // public static IMountSpriteApplicationContext Run([NotNull] Type rootModuleType,[NotNull]IServiceCollection services)
        // {
        //     return new MountSpriteApplicationContext(rootModuleType, services);
        // }
    }
}