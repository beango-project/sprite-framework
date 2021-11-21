using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite
{
    public class SpriteApplicationCreateOptions
    {
        public SpriteApplicationCreateOptions([NotNull] IServiceCollection services)
        {
            Services = services;
        }

        [NotNull]
        public IServiceCollection Services { get; }
    }
}