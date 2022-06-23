using System;
using FastExpressionCompiler.LightExpression;
using Sprite.Data;
using Sprite.Modular;

namespace Sprite.AspNetCore.Mvc
{
    public class SpriteAspNetCoreMvcConfig : ModuleConfig
    {
        public override Type[] ImportModules()
        {
            return new[] { typeof(SpriteAspNetCoreModule), typeof(SpriteDataModule) };
        }

        public override void Configure()
        {
            ImportModules();
        }
    }
}