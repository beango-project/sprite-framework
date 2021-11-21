using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sprite.Data.Transaction;

namespace Sprite.Security.Identity
{
    public class SpriteSecurityStampValidator<TUser> : SecurityStampValidator<TUser> where TUser : class
    {
        private readonly ILogger<SpriteSecurityStampValidator<TUser>> _logger;

        public SpriteSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<TUser> signInManager, ISystemClock clock, ILoggerFactory logger) : base(
            options, signInManager, clock, logger)
        {
            _logger = logger.CreateLogger<SpriteSecurityStampValidator<TUser>>();
        }

        [Transactional(Propagation.Auto)]
        public override async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            try
            {
                await base.ValidateAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError(exception: e, message: "{context}", context);
                throw new AuthenticationException("验证失败，请重新登录！");
            }
        }
    }
}