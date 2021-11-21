using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    [Register(ServiceLifetime.Transient)]
    public class SpriteAntiforgeryCookieProvider
    {
        private readonly SpriteAntiforgeryOptions _antiforgeryOptions;
        private readonly IOptionsSnapshot<CookieAuthenticationOptions> _options;

        public SpriteAntiforgeryCookieProvider(IOptionsSnapshot<CookieAuthenticationOptions> options, IOptions<SpriteAntiforgeryOptions> antiForgeryOptions)
        {
            _options = options;
            _antiforgeryOptions = antiForgeryOptions.Value;
            _options.Value.Cookie.Name = _antiforgeryOptions.CookieAuthenticationName;
        }

        public virtual CookieBuilder GetAntiForgeryCookie()
        {
            return _antiforgeryOptions.Cookie;
        }


        public virtual string GetAuthCookieName()
        {
            if (_antiforgeryOptions.CookieAuthenticationName is null)
            {
                return null;
            }

            var cookie = _options.Get(_antiforgeryOptions.CookieAuthenticationName)?.Cookie;
            return cookie?.Name;
        }

        public virtual string GetAntiForgeryCookieName()
        {
            return _antiforgeryOptions.CookieName;
        }
    }
}