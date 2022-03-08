using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        public SpriteConfigurationOptions Configuration = new SpriteConfigurationOptions();

        [CanBeNull]
        public IHostEnvironment Environment { get; }
    }
}