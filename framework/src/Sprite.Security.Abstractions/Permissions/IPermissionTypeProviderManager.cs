using System.Collections.Generic;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 权限类型管理器
    /// </summary>
    public interface IPermissionTypeProviderManager
    {
        IReadOnlyList<IPermissionTypeProvider> Providers { get; }
    }
}