using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Tests
{
    public abstract class SpriteTestsServiceProvider
    {
        protected abstract IServiceProvider ServiceProvider { get; }

        protected virtual T GetService<T>() => ServiceProvider.GetService<T>();

        protected virtual object GetService(Type serviceType) => ServiceProvider.GetService(serviceType);

        protected virtual object GetRequiredService(Type serviceType) => ServiceProvider.GetRequiredService(serviceType);

        protected virtual T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();
    }
}