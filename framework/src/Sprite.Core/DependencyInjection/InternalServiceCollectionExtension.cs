using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Sprite;
using Sprite.Context;
using Sprite.DynamicProxy;
using Sprite.Modular;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InternalServiceCollectionExtension
    {
        public static void AddCoreService(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging(x => x.AddSerilog());
        }

        public static void AddSpriteService(this IServiceCollection services, Type root, SpriteApplicationCreateOptions options)
        {
            if (!services.Any(s => s.ServiceType == typeof(IConfiguration)))
            {
                services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), ConfigurationGenerator.Create(options: options.Configuration)));
            }

            var moduleStore = new ModuleStore();
            services.TryAddSingleton<IModuleStore>(moduleStore);

            var moduleLoader = new ModuleLoader(services, root);
            services.TryAddSingleton<IModuleLoader>(moduleLoader);
            services.AddFromAssemblyOf<ISpriteApplicationContext>();

            services.Configure<InvocationOptions>(x => { x.Interceptors.Clear(); });
            services.AddOptions<InterceptorOptions>();
        }
    }
}