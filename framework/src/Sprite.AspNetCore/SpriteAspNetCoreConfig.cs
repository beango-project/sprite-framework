using Sprite.Modular;
using Sprite.Web;

namespace Sprite.AspNetCore
{
    public class SpriteAspNetCoreConfig : ModuleConfig
    {
        public override void Configure()
        {
            ImportModules(typeof(SpriteWebModule));
        }
    }
}