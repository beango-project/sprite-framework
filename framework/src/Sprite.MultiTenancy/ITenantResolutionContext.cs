using JetBrains.Annotations;

namespace Sprite.MultiTenancy
{
    public interface ITenantResolutionContext
    {
        [CanBeNull]
        string TenantIdOrName { get; set; }

        bool Handled { get; set; }

        bool Resolved();
    }
}