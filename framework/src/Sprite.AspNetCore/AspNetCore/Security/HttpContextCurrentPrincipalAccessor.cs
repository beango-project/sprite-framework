using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Sprite.Security.Claims;

namespace Sprite.AspNetCore.AspNetCore.Security
{
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