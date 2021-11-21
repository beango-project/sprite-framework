using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Sprite.Security.Identity
{
    public class IdentityUserClaimsPrincipalFactory<TIdentityUser, TIdentityRole, TUserKey,TRoleKey> : UserClaimsPrincipalFactory<TIdentityUser, TIdentityRole>
        where TIdentityUser : IdentityUser<TUserKey>
        where TIdentityRole : IdentityRole<TRoleKey>
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>

    {
        public IdentityUserClaimsPrincipalFactory(UserManager<TIdentityUser> userManager, RoleManager<TIdentityRole> roleManager, IOptions<IdentityOptions> options) : base(
            userManager, roleManager, options)
        {
        }
    }
}