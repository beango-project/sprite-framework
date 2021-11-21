using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Sprite.MultiTenancy
{
    public class TenantResolver : ITenantResolver
    {
        private readonly MultiTenancyOptions _options;

        public TenantResolver(IOptions<MultiTenancyOptions> options)
        {
            _options = options?.Value;
        }

        public async Task<TenantResolveResult> ResolveTenantIdOrNameAsync()
        {
            var result = new TenantResolveResult();
            var context = new TenantResolutionContext();
            foreach (var strategy in _options.ResolutionStrategies)
            {
                await strategy.ResolveAsync(context);
                if (context.Resolved())
                {
                    result.TenantIdOrName = context.TenantIdOrName;
                    break;
                }
            }

            return result;
        }
    }
}