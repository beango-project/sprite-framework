using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sprite.Security.Permissions
{
    public class PermissionsProvideContext : IPermissionsProvideContext
    {
        internal Dictionary<string, PermissionGroup> Groups { get; }

        public PermissionsProvideContext()
        {
            Groups = new Dictionary<string, PermissionGroup>();
        }

        public PermissionGroup AddGroup([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            if (Groups.ContainsKey(name))
            {
                throw new Exception($"There is already an existing permission group with name: {name}");
            }

            return Groups[name] = new PermissionGroup(name);
        }
    }
}