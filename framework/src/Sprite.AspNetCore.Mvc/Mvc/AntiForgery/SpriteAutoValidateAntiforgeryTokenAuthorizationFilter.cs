using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sprite.AspNetCore.Mvc.Abstractions;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.AspNetCore.Mvc.AntiForgery
{
    [Component(ServiceLifetime.Transient)]
    public class SpriteAutoValidateAntiforgeryTokenAuthorizationFilter : SpriteValidateAntiforgeryTokenAuthorizationFilter
    {
        private readonly SpriteAntiforgeryOptions _antiForgeryOptions;
        private readonly ILogger<SpriteValidateAntiforgeryTokenAuthorizationFilter> _logger;
        private readonly IAuthenticationSchemeProvider _authentication;

        public SpriteAutoValidateAntiforgeryTokenAuthorizationFilter(IAntiforgery antiforgery, SpriteAntiforgeryCookieProvider antiForgeryCookieProvider,
            IOptions<SpriteAntiforgeryOptions> antiForgeryOptions, ILogger<SpriteValidateAntiforgeryTokenAuthorizationFilter> logger, IAuthenticationSchemeProvider authentication) : base(
            antiforgery,
            antiForgeryCookieProvider, logger, authentication)
        {
            
            _antiForgeryOptions = antiForgeryOptions.Value;
        }

        protected override bool ShouldValidate(AuthorizationFilterContext context)
        {
            if (!_antiForgeryOptions.AutoValidate)
            {
                return false;
            }

            if (context.ActionDescriptor.IsControllerAction())
            {
                var controllerType = context.ActionDescriptor
                    .AsControllerActionDescriptor()
                    .ControllerTypeInfo
                    .AsType();

                if (!_antiForgeryOptions.AutoValidateFilter(controllerType))
                {
                    return false;
                }
            }

            //If the requested method ignores the verification method, then the automatic verification is skipped
            if (context.HttpContext.Request.Method.ToLowerInvariant().IsIn(_antiForgeryOptions.AutoValidateIgnoredHttpMethods))
            {
                return false;
            }

            return base.ShouldValidate(context);
        }
    }
}