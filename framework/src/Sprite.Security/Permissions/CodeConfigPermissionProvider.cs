using System.Collections.Concurrent;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 代码配置权限提供程序
    /// </summary>
    public class CodeConfigPermissionProvider : IPermissionProvider
    {
        private ConcurrentDictionary<string, Permission> _permissions=new ConcurrentDictionary<string, Permission>();

        /// <summary>
        /// 可以自定义提供程序的key
        /// </summary>
        private string _providerKey = "C";

        public string CurrentProviderKey => _providerKey;

        public bool IsPersistent => false;
        public bool Initialized => _initialize;

        private bool _initialize;

        public CodeConfigPermissionProvider()
        {
            _permissions.TryAdd("猪头三", new Permission()
            {
                Name = "猪头三"
            });
        }
        
        /// <summary>
        /// 加载所有权限
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void LoadingPermissions()
        {
            // _permissions由反射装配
            _initialize = true;
        }

        public Permission Get(string name)
        {
            _permissions.TryGetValue(name, out var permission);
            return permission;
        }

        public void DefinePermissions(IPermissionsProvideContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}