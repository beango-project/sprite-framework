using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Sprite.Security.Authorization
{
    public interface IDynamicAuthorizationPolicy : IAuthorizationPolicyProvider
    {
    }
}