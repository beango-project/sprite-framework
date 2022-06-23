using System;
using Sprite.Modular;

namespace Sprite.Data.EntityFrameworkCore
{
    public class SpriteEfCoreModuleConfig : ModuleConfig
    {
        public override Type[] ImportModules()
        {
            return new[] { typeof(SpriteDataModule) };
        }
    }
    
}