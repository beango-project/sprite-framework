using System;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection;

namespace Sprite
{
    public static class ApplicationServices
    {
        private static IServiceProvider ServiceProvider { get; set; }


        static ApplicationServices()
        {
        }

        internal static void Initialize(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            ServiceProvider = serviceProvider;
        }

        public static object GetInstance(Type type) => ServiceProvider.GetRequiredService(type);
    }
}