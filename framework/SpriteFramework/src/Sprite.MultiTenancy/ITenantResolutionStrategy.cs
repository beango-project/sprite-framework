using System.Threading.Tasks;

namespace Sprite.MultiTenancy
{
    public interface ITenantResolutionStrategy
    {
        Task ResolveAsync(ITenantResolutionContext context);
    }
}