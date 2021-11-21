using System;
using Sprite.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionSecurityExtensions
    {
        public static void AddSecurity(this IServiceCollection services, Action<SecurityOptions> options)
        {
            var securityOptions = new SecurityOptions(services);
            options?.Invoke(securityOptions);
            services.Configure(options);
        }
    }
}