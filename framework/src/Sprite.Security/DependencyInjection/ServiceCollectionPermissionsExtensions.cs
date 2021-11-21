using System;
using Microsoft.Extensions.Options;
using Sprite.Security.Permissions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionPermissionsExtensions
    {
        
        public static PermissionBuild AddPermissions(this IServiceCollection services, Action<PermissionOptions> action = null)
        {
            services.Configure<PermissionOptions>(options => { action?.Invoke(options); });
            services.AddSingleton<IPermissionProviderManager, PermissionProviderManager>();
            services.AddSingleton<IPermissionTypeProviderManager>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<PermissionOptions>>();
                return new PermissionTypeProviderManager(options, sp);
            });
            services.AddScoped<PermissionManager>();
            return new PermissionBuild(services);
        }
    }
}