using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Context
{
    public interface ISpriteApplicationContext : IDisposable
    {
        Type RootModuleType { get; }

        IServiceCollection Services { get; }

        IServiceProvider ServiceProvider { get; }

        void Shutdown();
    }
}