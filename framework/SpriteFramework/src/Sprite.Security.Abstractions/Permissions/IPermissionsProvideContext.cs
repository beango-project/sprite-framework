using JetBrains.Annotations;

namespace Sprite.Security.Permissions
{
    public interface IPermissionsProvideContext
    {
        PermissionGroup AddGroup([NotNull] string name);
    }
}