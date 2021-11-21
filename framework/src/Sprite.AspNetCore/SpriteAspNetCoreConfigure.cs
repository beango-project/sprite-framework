using Sprite.Modular;
using Sprite.Web;

namespace Sprite.AspNetCore
{
    public class SpriteAspNetCoreConfigure : ModuleConfigure
    {
        public override void Configure()
        {
            ImportModules(typeof(SpriteWebModule));
        }
    }
}