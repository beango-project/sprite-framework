namespace Sprite.Security.Permissions
{
    public abstract class PermissionProvider : IPermissionProvider
    {
        public string CurrentProviderKey { get; }
        public bool IsPersistent { get; }
        public bool Initialized { get; }

        public void LoadingPermissions()
        {
            throw new System.NotImplementedException();
        }

        public Permission Get(string name)
        {
            throw new System.NotImplementedException();
        }

        public abstract void DefinePermissions(IPermissionsProvideContext context);
    }
}