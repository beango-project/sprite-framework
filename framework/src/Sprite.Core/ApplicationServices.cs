using System;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection;

namespace Sprite
{
    public static class ApplicationServices
    {
        private static IServiceProvider ServiceProvider { get; set; }
        private static bool _initialized;

        static ApplicationServices()
        {
        }

        internal static void Initialize(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            if (_initialized)
            {
                throw new Exception($"{nameof(ApplicationServices)} has been initialized before!");
            }

            ServiceProvider = serviceProvider;
        }

        public static object GetInstance(Type type) => ServiceProvider.GetRequiredService(type);
    }
}