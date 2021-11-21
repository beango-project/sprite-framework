using System;
using Sprite.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionDynamicApiExtension
    {
        public static void AddDynamicApi(this IServiceCollection services, Action<AspNetCoreMvcOptions> actionOptions)
        {
            services.Configure(actionOptions);
        }
    }
}