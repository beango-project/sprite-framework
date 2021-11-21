using System.Collections.Generic;
using System.Security.Claims;

namespace Sprite.Security.Permissions
{
    public class Permission
    {
        public const string ClaimType = "Permission";

        /// <summary>
        /// 权限的唯一名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 权限类别
        /// </summary>
        public string Category { get; set; }

        public virtual Permission ParentPermission { get; }
        
        /// <summary>
        /// 权限类型
        /// </summary>
        public virtual IPermissionType PermissionType { get; }

        // public IEnumerable<Permission> ImpliedBy { get; }
    }
}