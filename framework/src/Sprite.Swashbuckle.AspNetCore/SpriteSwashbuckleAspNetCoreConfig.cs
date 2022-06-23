using System;
using Sprite.AspNetCore.Mvc;
using Sprite.Modular;

namespace Sprite.Swashbuckle.AspNetCore
{
    /// <summary>
    /// Sprite swashbuckle AspNetCore module configure
    /// </summary>
    public class SpriteSwashbuckleAspNetCoreConfig : ModuleConfig
    {
        public override Type[] ImportModules()
        {
            return new[] { typeof(SpriteAspNetCoreMvcModule) };
        }
    }
}