namespace Sprite.Security.Permissions
{
    public class PermissionType : IPermissionType
    {
        public string Key { get; }
        public string Type { get; }

        public PermissionType(string key, string type)
        {
            Key = key;
            Type = type;
        }
    }
}