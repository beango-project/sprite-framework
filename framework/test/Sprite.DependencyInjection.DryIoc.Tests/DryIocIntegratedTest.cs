using Sprite.Tests;

namespace Sprite.DependencyInjection.DryIoc.Tests
{
    public class DryIocIntegratedTest : SpriteIntegratedTest<SpriteDryIocTestModule>
    {
        protected override void SetAppCreateOptions(SpriteApplicationCreateOptions options)
        {
            options.UseDryIoc();
        }
    }
}