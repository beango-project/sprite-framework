using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.DependencyInjection.DryIoc
{
    public static class SpriteGenericHostBuilderForDryIocExtensions
    {
        public static IHostBuilder UseSpriteServiceProviderFactory(this IHostBuilder hostBuilder)
        {
            var container = DryIocServiceProviderBuilder.Build();
            return hostBuilder.UseServiceProviderFactory(new DryIocServiceProviderFactory(container));
        }
    }
}