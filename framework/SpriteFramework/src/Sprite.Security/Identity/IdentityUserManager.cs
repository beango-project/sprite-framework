using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sprite.Data.Exceptions;

namespace Sprite.Security.Identity
{
    public class IdentityUserManager<TIdentityUser, TKey> : UserManager<TIdentityUser>
        where TIdentityUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public IdentityUserManager(IUserStore<TIdentityUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TIdentityUser> passwordHasher,
            IEnumerable<IUserValidator<TIdentityUser>> userValidators, IEnumerable<IPasswordValidator<TIdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TIdentityUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators,
            passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public virtual async Task<IdentityResult> CreateAsync(TIdentityUser user, string password, bool validatePassword)
        {
            var result = await UpdatePasswordHash(user, password, validatePassword);
            if (!result.Succeeded)
            {
                return result;
            }

            return await CreateAsync(user);
        }

        public virtual async Task<TIdentityUser> GetByIdAsync(TKey id)
        {
            var user = await Store.FindByIdAsync(id.ToString(), CancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(typeof(TIdentityUser), id);
            }

            return user;
        }

        public virtual async Task<IdentityResult> SetRolesAsync([NotNull] TIdentityUser user,
            [NotNull] IEnumerable<string> roleNames)
        {
            Check.NotNull(user, nameof(user));
            Check.NotNull(roleNames, nameof(roleNames));

            var currentRoleNames = await GetRolesAsync(user);

            var result = await RemoveFromRolesAsync(user, currentRoleNames.Except(roleNames).Distinct());
            if (!result.Succeeded)
            {
                return result;
            }

            result = await AddToRolesAsync(user, roleNames.Except(currentRoleNames).Distinct());
            if (!result.Succeeded)
            {
                return result;
            }

            return IdentityResult.Success;
        }
    }
}