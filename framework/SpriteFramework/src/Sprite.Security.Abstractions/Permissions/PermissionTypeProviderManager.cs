using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sprite.Security.Permissions
{
    public class PermissionTypeProviderManager : IPermissionTypeProviderManager
    {
        public IReadOnlyList<IPermissionTypeProvider> Providers => _permissionTypes.ToImmutableList();

        // private readonly Lazy<List<IPermissionTypeProvider>> _lazy;

        private readonly List<IPermissionTypeProvider> _permissionTypes;
        protected PermissionOptions Options;

        public PermissionTypeProviderManager(IOptions<PermissionOptions> options, IServiceProvider serviceProvider)
        {
            Options = options.Value;
            _permissionTypes = options.Value.PermissionTypeProviders.Select(c => serviceProvider.GetRequiredService(c) as IPermissionTypeProvider).ToList();
        }
    }
}