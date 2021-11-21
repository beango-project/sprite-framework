using System.Threading.Tasks;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 权限类型提供者
    /// </summary>
    public interface IPermissionTypeProvider
    {
        IPermissionType Type { get; }
        
        IPermissionStore Store { get; }

        Task<PermissionGrantedResult> CheckAsync(PermissionCheckContext context);
    }
}