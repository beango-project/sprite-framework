using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 权限管理器，用于从权限提供器中获取权限
    /// </summary>
    public class PermissionManager
    {
        /// <summary>
        /// 权限提供者,用于从xml，json，数据库等地方加载并获取定义权限
        /// </summary>
        /// <returns></returns>
        private readonly IPermissionProviderManager _permissionProviderManager;

        private readonly IPermissionTypeProviderManager _permissionTypeProviderManager;


        public PermissionManager(IPermissionProviderManager permissionProviderManager, IPermissionTypeProviderManager permissionTypeProviderManager)
        {
            _permissionProviderManager = permissionProviderManager;
            _permissionTypeProviderManager = permissionTypeProviderManager;
        }

        protected virtual Permission GetPermission(string name)
        {
            return _permissionProviderManager.Get(name);
        }

        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="name"></param>
        /// <param name="providerKey"></param>
        /// <param name="isGranted"></param>
        public async Task SetPermission(string name, string providerKey, string claimValue, bool isGranted = false)
        {
            var permissionTypeProvider = _permissionTypeProviderManager.Providers.SingleOrDefault(x => x.Type.Key == providerKey);
            if (permissionTypeProvider != null)
            {
                var permission = _permissionProviderManager.Get(name);
                if (permission != null)
                {
                    await permissionTypeProvider.Store.SetPermissionsAsync(permission.Name, permissionTypeProvider.Type.Key, claimValue);
                }
            }
        }


        public virtual async Task<bool> IsGrantedAsync([NotNull] string name, ClaimsPrincipal claimsPrincipal)
        {
            var permission = this.GetPermission(name);
            var context = new PermissionCheckContext(permission, claimsPrincipal);
            bool isGranted = false;
            foreach (var permissionTypeProvider in _permissionTypeProviderManager.Providers)
            {
                var result = await permissionTypeProvider.CheckAsync(context);

                if (result == PermissionGrantedResult.Granted)
                {
                    isGranted = true;
                }
                else if (result == PermissionGrantedResult.Prohibited)
                {
                    return false;
                }
            }

            return isGranted;
        }
    }
}