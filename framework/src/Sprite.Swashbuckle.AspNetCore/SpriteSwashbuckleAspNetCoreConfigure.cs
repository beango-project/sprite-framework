using Sprite.AspNetCore.Mvc;
using Sprite.Modular;

namespace Sprite.Swashbuckle.AspNetCore
{
    /// <summary>
    /// Sprite swashbuckle AspNetCore module configure
    /// </summary>
    public class SpriteSwashbuckleAspNetCoreConfigure : ModuleConfigure
    {
        /// <summary>
        /// configure import depends module
        /// </summary>
        public override void Configure()
        {
            ImportModules(typeof(SpriteAspNetCoreMvcSpriteModule));
        }
    }
}