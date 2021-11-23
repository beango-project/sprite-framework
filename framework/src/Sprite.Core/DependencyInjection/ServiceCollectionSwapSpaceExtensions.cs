using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.DependencyInjection
{
    public static class ServiceCollectionSwapSpaceExtensions
    {
        public static SwapSpace TryAddSwapSpace(this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType == typeof(SwapSpace)))
            {
                return services.GetSingletonInstance<SwapSpace>();
            }

            return services.AddSwapSpace();
        }

        public static SwapSpace AddSwapSpace(this IServiceCollection services)
        {
            return services.AddSwapSpace(new SwapSpace());
        }

        public static SwapSpace AddSwapSpace(this IServiceCollection services, SwapSpace swap)
        {
            if (services.Any(s => s.ServiceType == typeof(SwapSpace)))
            {
                throw new Exception("An swap space is registered before for type: " + typeof(SwapSpace).AssemblyQualifiedName);
            }

            //Add to the beginning for fast retrieve
            services.Insert(0, ServiceDescriptor.Singleton(typeof(SwapSpace), swap));
            services.Insert(0, ServiceDescriptor.Singleton(typeof(ISwapSpace), swap));

            return swap;
        }

        public static void AddInSwapSpace<T>(this IServiceCollection services, object value = null)
        {
            var swapSpace = GetSwapSpace(services);

            swapSpace.Add<T>(value);
        }

        public static void TryAddInSwapSpace<T>(this IServiceCollection services, object value = null)
        {
            var swapSpace = GetSwapSpace(services);

            swapSpace.TryAdd<T>(value);
        }

        public static T GetFromSwapSpace<T>(this IServiceCollection services)
            where T : class
        {
            return services.GetSingletonInstance<SwapSpace>()?.Get<T>();
        }

        public static ISwapSpace GetSwapSpace(this IServiceCollection services)
        {
            var swapSpace = services.GetSingletonInstance<SwapSpace>();
            if (swapSpace == null)
            {
                throw new Exception("SwapSpace was not found, please register SwapSpace first");
            }

            return swapSpace;
        }
    }
}