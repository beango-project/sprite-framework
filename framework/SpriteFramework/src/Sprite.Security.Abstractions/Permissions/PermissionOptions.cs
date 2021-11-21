using System;
using System.Collections.Generic;

namespace Sprite.Security.Permissions
{
    public class PermissionOptions
    {
        public List<IPermissionProvider> PermissionProviders { get; }
        public List<Type> PermissionTypeProviders { get; set; }

        public PermissionOptions()
        {
            PermissionTypeProviders = new List<Type>();
            PermissionProviders = new List<IPermissionProvider>();
        }

        public void AddPermissionProvider<TPermissionProvider>() where TPermissionProvider: IPermissionProvider
        {
            
        }
    }
}