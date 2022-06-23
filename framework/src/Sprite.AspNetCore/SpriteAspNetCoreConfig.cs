using System;
using Sprite.Modular;
using Sprite.Web;

namespace Sprite.AspNetCore
{
    public class SpriteAspNetCoreConfig : ModuleConfig
    {
        public override Type[] ImportModules()
        {
            return new[] { typeof(SpriteWebModule) };
        }
    }
}