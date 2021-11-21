using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sprite.Security.Permissions
{
    public interface IPermissionStore
    {
        Task<IQueryable<Permission>> GetPermissionsAsync(Func<Permission, bool> filter, CancellationToken cancellationToken = default);

        Task<bool> IsGrantedAsync([NotNull] string name,[NotNull] string ClaimValue, CancellationToken cancellationToken = default);

        Task SetPermissionsAsync(string permissionName, string providerKey, string ClaimValue, CancellationToken cancellationToken = default);
    }
}