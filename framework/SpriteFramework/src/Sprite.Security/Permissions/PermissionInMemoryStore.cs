using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sprite.Security.Permissions
{
    public class PermissionInMemoryStore : IPermissionStore
    {
        private ConcurrentDictionary<string, Permission> _maps = new ConcurrentDictionary<string, Permission>();

        private ConcurrentDictionary<string, Permission> _userMaps = new ConcurrentDictionary<string, Permission>();

        public PermissionInMemoryStore()
        {
            _maps.TryAdd("猪头三", new Permission()
            {
                Name = "猪头三"
            });

            _maps.TryAdd("小偷", new Permission()
            {
                Name = "小偷"
            });
        }

        public Task<IQueryable<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Permission>> GetPermissionsAsync(Func<Permission, bool> filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsGrantedAsync(string name, string permissionType, CancellationToken cancellationToken = default)
        {
            if (_maps.ContainsKey(name) && _userMaps.ContainsKey(permissionType))
            {
                if (_maps[name].Name == _userMaps[permissionType].Name)
                {
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }

            return Task.FromResult(false);
        }

        public Task SetPermissionsAsync(string permissionName, string providerKey, string claimValue, CancellationToken cancellationToken = default)
        {
            _userMaps.TryAdd(claimValue, _maps[permissionName]);
            return Task.CompletedTask;
        }

        // public Task SetPermissionsAsync(string ClaimValue, Permission permission)
        // {
        //     _userMaps.TryAdd(ClaimValue, permission);
        //     return Task.CompletedTask;
        // }
    }
}