using Sprite.Modular;

namespace Sprite.Data.EntityFrameworkCore
{
    public class SpriteEfCoreModuleConfig : ModuleConfig
    {
        public override void Configure()
        {
            ImportModules(typeof(SpriteDataModule));
        }
    }
}