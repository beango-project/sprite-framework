using Sprite.Modular;

namespace Sprite.Data.EntityFrameworkCore
{
    public class SpriteEfCoreModuleConfigure : ModuleConfigure
    {
        public override void Configure()
        {
            ImportModules(typeof(SpriteDataModule));
        }
    }
}