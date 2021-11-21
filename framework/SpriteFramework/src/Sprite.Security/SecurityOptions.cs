using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sprite.Security.Authorization;
using Sprite.Security.Permissions;

namespace Sprite.Security
{
    public class SecurityOptions
    {
        public SecurityOptions(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public PermissionBuild AddPermissions(Action<PermissionOptions> action = null)
        {
            Services.Configure<PermissionOptions>(options => { action?.Invoke(options); });
            Services.TryAddSingleton<IPermissionProviderManager, PermissionProviderManager>();
            Services.AddAuthorizationCore();
            Services.TryAddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicy>();
            Services.TryAddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
            Services.TryAddTransient<DefaultAuthorizationPolicyProvider>();
            Services.TryAddSingleton<IPermissionTypeProviderManager>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<PermissionOptions>>();
                return new PermissionTypeProviderManager(options, sp);
            });
            Services.TryAddSingleton<PermissionManager>();
            return new PermissionBuild(Services);
        }
    }
}