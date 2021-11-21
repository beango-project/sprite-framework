namespace Sprite.Security.Permissions
{
    public interface IPermissionType
    {
        string Key { get; }
        string Type { get; }
    }
}