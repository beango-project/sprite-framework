using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Sprite.Security.Identity
{
    public class IdentityRoleManager<TIdentityRole, TIdentityUser,TUserKey,TRoleKey> : RoleManager<TIdentityRole>
        where TIdentityUser: IdentityUser<TUserKey>
        where TIdentityRole : IdentityRole<TRoleKey>
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        public IdentityRoleManager(IRoleStore<TIdentityRole> store, IEnumerable<IRoleValidator<TIdentityRole>> roleValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, ILogger<RoleManager<TIdentityRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }

        public override async Task<IdentityResult> DeleteAsync(TIdentityRole role)
        {
            return await base.DeleteAsync(role);
        }
    }
}