using System.Security.Claims;
using JetBrains.Annotations;

namespace Sprite.Security.Permissions
{
    public class PermissionCheckContext
    {
        [NotNull]
        public Permission Permission { get; }

        [CanBeNull]
        public ClaimsPrincipal Principal { get; }

        public PermissionCheckContext(
            [NotNull] Permission permission,
            [CanBeNull] ClaimsPrincipal principal)
        {
            Check.NotNull(permission, nameof(permission));

            Permission = permission;
            Principal = principal;
        }
    }
}