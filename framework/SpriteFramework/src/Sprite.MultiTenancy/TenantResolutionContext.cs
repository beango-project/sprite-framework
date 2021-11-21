namespace Sprite.MultiTenancy
{
    public class TenantResolutionContext : ITenantResolutionContext
    {
        public string TenantIdOrName { get; set; }
        public bool Handled { get; set; }

        public bool Resolved() => Handled || TenantIdOrName != null;
    }
}