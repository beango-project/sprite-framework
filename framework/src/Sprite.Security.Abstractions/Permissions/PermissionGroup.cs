using System.Collections.Generic;
using System.Collections.Immutable;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 权限组
    /// </summary>
    public class PermissionGroup
    {
        public string Name { get; }

        public IReadOnlyList<Permission> Permissions => _permissions.ToImmutableList();
        private IList<Permission> _permissions=new List<Permission>();

        public PermissionGroup(string name)
        {
            Name = name;
        }

        public virtual Permission AddPermission(string name)
        {
            var permission = new Permission()
            {
                Name = name
            };
            _permissions.Add(permission);
            return permission;
        }
    }
}