using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Repositories;

namespace Sprite.Security.Permissions
{
    // public class PermissionStore<TPermission>: IPermissionStore<TPermission> where TPermission : class
    // {
    //     public Task<IQueryable<TPermission>> GetPermissionsAsync()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public Task<IQueryable<TPermission>> GetPermissionsAsync(Func<TPermission, bool> filter)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public Task<bool> IsGrantedAsync(TPermission permission)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public Task<bool> IsGrantedAsync(string name, string permissionType = null)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }

    // public abstract class PermissionStore : IPermissionStore
    // {
    //     private readonly IRepository<PermissionGrantDetail, long> _permissionGrantRepository;
    //
    //     protected PermissionStore(IRepository<PermissionGrantDetail, long> permissionGrantRepository)
    //     {
    //         _permissionGrantRepository = permissionGrantRepository;
    //     }
    //
    //
    //     public Task<IQueryable<Permission>> GetPermissionsAsync(Func<Permission, bool> filter, CancellationToken cancellationToken = default)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public virtual Task<bool> IsGrantedAsync(string name, string ClaimValue, CancellationToken cancellationToken = default)
    //     {
    //         Check.NotNull(name, nameof(name));
    //         Check.NotNull(ClaimValue, ClaimValue);
    //
    //         return Task.FromResult(_permissionGrantRepository.GetAsync(x => x.Name == name && x.ClaimValue == ClaimValue, cancellationToken) != null);
    //     }
    //
    //     public abstract Task SetPermissionsAsync(string permissionName, string providerKey, string ClaimValue, CancellationToken cancellationToken = default);
    // }
}