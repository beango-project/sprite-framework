using Sprite.Modular;

namespace Sprite.AspNetCore.Mvc
{
    public class SpriteAspNetCoreMvcConfigure : ModuleConfigure
    {
        public override void Configure()
        {
            ImportModules(typeof(SpriteAspNetCoreModule));
        }
    }
}