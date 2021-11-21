using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Sprite.Security.Permissions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 构建权限
    /// 用于在依赖注入时进行链式调用
    /// </summary>
    public class PermissionBuild
    {
        public PermissionBuild(IServiceCollection services)
        {
            Services = services;
        }
        //
        // public ImmutableDictionary<Type, Type> PermissionTypeStoreMap => _permissionTypeStoreMap.ToImmutableDictionary();
        // private Dictionary<Type, Type> _permissionTypeStoreMap { get; } = new Dictionary<Type, Type>();

        public List<IPermissionTypeProvider> PermissionTypes { get; protected set; }

        public IServiceCollection Services { get; }

        public PermissionBuild AddPermissionType<TProviderType, TStore>() where TProviderType : IPermissionTypeProvider where TStore : IPermissionStore
        {
            Services.Configure<PermissionOptions>(options => { options.PermissionTypeProviders.AddIfNotContains(typeof(TProviderType)); });
            Services.AddScoped(typeof(IPermissionTypeProvider),typeof(TProviderType));
           
            Services.AddScoped(typeof(TStore));
            Services.AddScoped(typeof(IPermissionStore), typeof(TStore));
            return this;
        }
    }
}