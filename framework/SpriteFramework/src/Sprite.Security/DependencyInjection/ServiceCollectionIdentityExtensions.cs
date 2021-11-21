using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Security.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionIdentityExtensions
    {
        public static IdentityBuilder AddSpriteIdentity<TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction = null)
            where TUser : class
            where TRole : class
        {
            //Register User
            var userType = typeof(TUser);
            var userMeetType = TypeHelper.GetMatchType(userType, typeof(Sprite.Security.Identity.IdentityUser<>));
            var userKey = userMeetType.GenericTypeArguments[0];
            var identityUserManager = typeof(IdentityUserManager<,>).MakeGenericType(userType, userKey);
            services.TryAddScoped(typeof(UserManager<>), identityUserManager);
            // services.TryAddScoped(typeof(IUserStore<TUser>), provider => provider.GetService(typeof(IdentityUserStore)));

            //Register Role
            var roleMeetType = TypeHelper.GetMatchType(typeof(TRole), typeof(Sprite.Security.Identity.IdentityRole<>));
            var roleKey = roleMeetType.GenericTypeArguments[0];
            var identityRoleManager = typeof(IdentityRoleManager<,,,>).MakeGenericType(typeof(TRole), userType, userKey, roleKey);
            services.TryAddScoped(typeof(RoleManager<>), identityRoleManager);
            // var makeGenericType = typeof(IdentityUserStore<,,,,,,,>).MakeGenericType(builder.UserType, builder.RoleType);


            services.TryAddScoped(typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(userType),
                typeof(IdentityUserClaimsPrincipalFactory<,,,>).MakeGenericType(userType, roleMeetType, userKey, roleKey));
            if (setupAction != null)
            {
                services.Configure<IdentityOptions>(setupAction);
            }

            services.AddScoped(typeof(SecurityStampValidator<TUser>), provider => provider.GetService(typeof(SpriteSecurityStampValidator<TUser>)));
            services.AddScoped(typeof(ISecurityStampValidator), provider => provider.GetService(typeof(SpriteSecurityStampValidator<TUser>)));

            return services.AddIdentityCore<TUser>().AddRoles<TRole>();
        }
    }
}