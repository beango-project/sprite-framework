using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Sprite.Security.Permissions
{
    public class PermissionProviderManager : IPermissionProviderManager
    {
        protected IDictionary<string, PermissionGroup> PermissionGroupDefinitions;
        protected IDictionary<string, Permission> PermissionDefinitions;

        public PermissionOptions Options;

        public Permission Get(string name)
        {
            if (!PermissionDefinitions.ContainsKey(name))
            {
                throw new Exception("Not found permission " + name);
            }

            return PermissionDefinitions[name];
        }


        public PermissionProviderManager(IOptions<PermissionOptions> options)
        {
            Options = options.Value;
            PermissionGroupDefinitions = LoadPermissionGroup();
            PermissionDefinitions = LoadPermissions();
        }

        protected virtual Dictionary<string, PermissionGroup> LoadPermissionGroup()
        {
            var context = new PermissionsProvideContext();

            var providers = Options.PermissionProviders;
            foreach (var provider in providers)
            {
                provider.DefinePermissions(context);
            }

            return context.Groups;
        }


        protected virtual Dictionary<string, Permission> LoadPermissions()
        {
            var permissions = new Dictionary<string, Permission>();

            foreach (var groupDefinition in PermissionGroupDefinitions.Values)
            {
                foreach (var permission in groupDefinition.Permissions)
                {
                    AddPermissionToDictionaryRecursively(permissions, permission);
                }
            }

            return permissions;
        }

        protected virtual void AddPermissionToDictionaryRecursively(Dictionary<string, Permission> permissions, Permission permission)
        {
            if (permissions.ContainsKey(permission.Name))
            {
                throw new Exception("Duplicate permission name: " + permission.Name);
            }

            permissions[permission.Name] = permission;

            // foreach (var child in permission.Children)
            // {
            //     AddPermissionToDictionaryRecursively(permissions, child);
            // }
        }
    }
}