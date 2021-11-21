using JetBrains.Annotations;

namespace Sprite.Security.Permissions
{
    /// <summary>
    /// 权限提供者
    /// </summary>
    public interface IPermissionProvider
    {
        string CurrentProviderKey { get; }

        /// <summary>
        /// 是持久化提供者，代表不用加载使用持久化设备进行查询啊
        /// </summary>
        bool IsPersistent { get; }

        /// <summary>
        /// 初始化，加载定义的权限以供后续获取
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// 加载权限
        /// </summary>
        void LoadingPermissions();

        Permission Get(string name);
        
        void DefinePermissions(IPermissionsProvideContext context);
        
    }
}