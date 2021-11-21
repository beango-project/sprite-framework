using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    [Register(ServiceLifetime.Transient)]
    public class AspNetCoreAntiForgeryManager : IAntiForgeryManager
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreAntiForgeryManager(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IOptions<SpriteAntiforgeryOptions> options)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
            Options = options.Value;
        }

        public SpriteAntiforgeryOptions Options { get; }

        public HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public virtual void SetCookie()
        {
            HttpContext.Response.Cookies.Append(
                Options.Cookie.Name,
                GenerateToken(),
                Options.Cookie.Build(HttpContext)
            );
        }

        public virtual string GenerateToken()
        {
            return _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext).RequestToken;
        }
    }
}