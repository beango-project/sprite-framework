using Microsoft.Extensions.DependencyInjection;

namespace Sprite.DependencyInjection.Stashbox
{
    public static class SpriteApplicationCreateOptionExtension
    {
        public static void UseStashBox(this SpriteApplicationCreateOptions options)
        {
            options.Services.AddStashbox(container => { container.Configure(config => { config.WithSpriteInjection(); }); });
        }
    }
}