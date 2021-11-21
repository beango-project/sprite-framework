using System.Security.Claims;
using System.Threading.Tasks;
using Sprite.Security.Identity;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 用户权限类型提供程序
    /// </summary>
    public class UserPermissionTypeProvider : PermissionTypeProvider
    {
        // private readonly IPermissionStore _store;
        public UserPermissionTypeProvider(IPermissionStore store) : base(store)
        {
        }

        public override IPermissionType Type { get; protected set; } = new PermissionType("User", nameof(UserPermissionTypeProvider));


        public override async Task<PermissionGrantedResult> CheckAsync(PermissionCheckContext context)
        {
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!context.Principal.IsAuthenticated() || userId is null)
            {
                return await Task.FromResult(PermissionGrantedResult.Prohibited);
            }

            return await Store.IsGrantedAsync(context.Permission.Name, userId)
                ? PermissionGrantedResult.Granted
                : PermissionGrantedResult.Unknown;
        }
    }
}