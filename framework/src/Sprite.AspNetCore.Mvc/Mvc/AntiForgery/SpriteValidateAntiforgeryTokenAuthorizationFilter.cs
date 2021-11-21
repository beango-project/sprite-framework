using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sprite.DependencyInjection.Attributes;
using Sprite.Security.Authorization;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    [Register(ServiceLifetime.Transient)]
    public class SpriteValidateAntiforgeryTokenAuthorizationFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
    {
        private readonly IAntiforgery _antiforgery;
        private readonly SpriteAntiforgeryCookieProvider _antiForgeryCookieProvider;
        private readonly IAuthenticationSchemeProvider _authentication;
        private readonly ILogger<SpriteValidateAntiforgeryTokenAuthorizationFilter> _logger;

        public SpriteValidateAntiforgeryTokenAuthorizationFilter(IAntiforgery antiforgery, SpriteAntiforgeryCookieProvider antiForgeryCookieProvider,
            ILogger<SpriteValidateAntiforgeryTokenAuthorizationFilter> logger, IAuthenticationSchemeProvider authentication)
        {
            _antiforgery = antiforgery;
            _antiForgeryCookieProvider = antiForgeryCookieProvider;
            _logger = logger;
            _authentication = authentication;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Check.NotNull(context, nameof(context));
            if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
            {
                _logger.LogInformation("Skipping the execution of current filter as its not the most effective filter implementing the policy " + typeof(IAntiforgeryPolicy));
                return;
            }


            if (ShouldValidate(context))
            {
                try
                {
                    await _antiforgery.ValidateRequestAsync(context.HttpContext);
                }
                catch (AntiforgeryValidationException exception)
                {
                    _logger.LogError(exception.Message, exception);
                    context.Result = new AntiforgeryValidationFailedResult();
                }
            }
        }

        protected virtual bool ShouldValidate(AuthorizationFilterContext context)
        {
            var authCookieName = _antiForgeryCookieProvider.GetAuthCookieName();
            var authorizeDataAuthenticationSchemes = _authentication.GetDefaultAuthenticateSchemeAsync().Result.Name;
            var dataAuthenticationSchemes = "AspNetCore."+authorizeDataAuthenticationSchemes;
            //Always perform antiforgery validation when request contains authentication cookie
            if (authCookieName != null && context.HttpContext.Request.Cookies.ContainsKey(dataAuthenticationSchemes))
            {
                return true;
            }

            var antiForgeryCookieName = _antiForgeryCookieProvider.GetAntiForgeryCookieName();

            //No need to validate if antiforgery cookie is not sent.
            //That means the request is sent from a non-browser client.
            //See https://github.com/aspnet/Antiforgery/issues/115
            if (antiForgeryCookieName != null && !context.HttpContext.Request.Cookies.ContainsKey(antiForgeryCookieName))
            {
                return false;
            }

            // Anything else requires a token.
            return true;
        }
    }
}