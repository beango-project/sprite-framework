﻿using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Context
{
    public class MountSpriteApplicationContext : SpriteApplicationContextBase, IMountSpriteApplicationContext
    {
        public MountSpriteApplicationContext([NotNull] Type rootModuleType, [NotNull] IServiceCollection services, [CanBeNull] Action<SpriteApplicationCreateOptions>
            options=null) : base(rootModuleType, services, options)
        {
            services.AddSingleton<IMountSpriteApplicationContext>(this);
        }

        public void Run(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            SetServiceProvider(serviceProvider);
            Initialize();
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