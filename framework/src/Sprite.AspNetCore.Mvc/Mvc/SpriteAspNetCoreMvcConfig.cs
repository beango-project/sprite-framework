using Sprite.Modular;

namespace Sprite.AspNetCore.Mvc
{
    public class SpriteAspNetCoreMvcConfig : ModuleConfig
    {
        public override void Configure()
        {
            ImportModules(typeof(SpriteAspNetCoreModule));
        }
    }
}