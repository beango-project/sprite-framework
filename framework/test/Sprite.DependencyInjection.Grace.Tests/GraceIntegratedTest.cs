using Sprite.Tests;

namespace Sprite.DependencyInjection.Grace.Tests;

public class GraceIntegratedTest: SpriteIntegratedTest<SpriteGraceTestModule>
{
    protected override void SetAppCreateOptions(SpriteApplicationCreateOptions options)
    {
        options.UseGrace();
    }
}