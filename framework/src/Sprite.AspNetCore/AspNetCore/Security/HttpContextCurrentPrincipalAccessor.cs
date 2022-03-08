using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection.Attributes;
using Sprite.Security.Claims;

namespace Sprite.AspNetCore.Security
{
    [Component(ServiceLifetime.Singleton)]
    public class HttpContextCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentPrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override ClaimsPrincipal GetCurrentPrincipal()
        {
            return _httpContextAccessor.HttpContext?.User ?? base.GetCurrentPrincipal();
        }
    }
}