﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection;
using Sprite.Modular;

namespace Sprite.AspNetCore
{
    public class SpriteAspNetCoreModule : Module
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddInSwapSpace<IApplicationBuilder>();
        }
    }
}