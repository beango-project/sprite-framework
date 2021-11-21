using System;
using JetBrains.Annotations;

namespace Sprite.Context
{
    public interface IMountSpriteApplicationContext : ISpriteApplicationContext
    {
        void Run([NotNull] IServiceProvider serviceProvider);
    }
}