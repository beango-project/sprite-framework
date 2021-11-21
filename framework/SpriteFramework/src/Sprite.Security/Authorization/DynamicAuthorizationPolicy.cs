using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Sprite.Security.Authorization
{
    public class DynamicAuthorizationPolicy : DefaultAuthorizationPolicyProvider, IDynamicAuthorizationPolicy
    {
        private readonly IOptions<AuthorizationOptions> _options;

        public DynamicAuthorizationPolicy([NotNull] [ItemNotNull] IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy != null)
            {
                return policy;
            }

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName)).Build();


                //TODO 如果真的有此权限，就将它添加进option里面进行缓存，这样就不用每次进来都创建它，而是提供默认授权直接检测 
                // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
                return policy;
            }

            return null;
        }
        
        
    }
}