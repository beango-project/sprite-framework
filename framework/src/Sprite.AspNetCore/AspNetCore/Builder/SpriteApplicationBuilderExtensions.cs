﻿using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sprite;
using Sprite.Context;
using Sprite.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class SpriteApplicationBuilderExtensions
    {
        public static void UseSprite([NotNull] this IApplicationBuilder app)
        {
            Check.NotNull(app, nameof(app));

            var swapSpace = app.ApplicationServices.GetRequiredService<SwapSpace>();
            swapSpace.Set<IApplicationBuilder>(app);

            var context = app.ApplicationServices.GetRequiredService<IMountSpriteApplicationContext>();
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStopping.Register(() => { context.Shutdown(); });

            applicationLifetime.ApplicationStopped.Register(() => { context.Dispose(); });

            context.Run(app.ApplicationServices);
            
            swapSpace.Remove<IApplicationBuilder>();
        }
    }
}