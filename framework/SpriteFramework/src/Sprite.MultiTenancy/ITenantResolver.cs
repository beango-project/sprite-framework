using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sprite.MultiTenancy
{
    public interface ITenantResolver
    {
        [NotNull]
        Task<TenantResolveResult> ResolveTenantIdOrNameAsync();
    }
}