using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 权限提供管理，管理定义的权限可从中获取
    /// </summary>
    public interface IPermissionProviderManager
    {
        /// <summary>
        /// 根据权限名称获取权限
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <returns>权限<see cref="Permission"/></returns>
        Permission Get([NotNull] string name);
    }
}