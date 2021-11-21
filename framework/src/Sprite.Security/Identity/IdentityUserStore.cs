using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Sprite.Data.Entities;
using Sprite.Data.Repositories;

namespace Sprite.Security.Identity
{
    public class IdentityUserStore<TUser, TUserKey,
        TRole, TRoleKey,
        TUserRole,
        TUserLogin, TUserLoginKey,
        TUserToken, TUserTokenKey,
        TUserClaim, TUserClaimKey,
        TRoleClaim, TRoleClaimKey> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>
        where TUser : IdentityUser<TUserKey>, new()
        where TUserKey : IEquatable<TUserKey>
        where TRole : IdentityRole<TRoleKey>, new()
        where TRoleKey : IEquatable<TRoleKey>
        where TUserRole : IdentityUserRole<TUserKey, TRoleKey>, new()
        where TUserLogin : IdentityUserLogin<TUserLoginKey, TUserKey>, new()
        where TUserLoginKey : IEquatable<TUserLoginKey>
        where TUserToken : IdentityUserToken<TUserTokenKey, TUserKey>, new()
        where TUserTokenKey : IEquatable<TUserTokenKey>
        where TUserClaim : IdentityUserClaim<TUserClaimKey, TUserKey>, new()
        where TUserClaimKey : IEquatable<TUserClaimKey>
        where TRoleClaim : IdentityRoleClaim<TRoleClaimKey, TRoleKey>, new()
        where TRoleClaimKey : IEquatable<TRoleClaimKey>

    {
        public IdentityUserStore(IRepository<TUser, TUserKey> userRepository, IRepository<TUserLogin, TUserLoginKey> userLoginRepository,
            IRepository<TRole, TRoleKey> roleRepository, IRepository<TUserRole> userRoleRepository, IRepository<TUserToken, TUserTokenKey> userTokenRepository,
            IRepository<TUserClaim, TUserClaimKey> userClaimRepository, IRepository<TRoleClaim, TRoleClaimKey> roleClaimRepository, ILookupNormalizer lookupNormalizer,
            ILogger<IdentityUserStore<TUser, TUserKey, TRole, TRoleKey, TUserRole, TUserLogin, TUserLoginKey, TUserToken, TUserTokenKey, TUserClaim,
                TUserClaimKey, TRoleClaim, TRoleClaimKey>> logger, IdentityErrorDescriber describer = null)
        {
            UserRepository = userRepository;
            UserLoginRepository = userLoginRepository;
            RoleRepository = roleRepository;
            UserRoleRepository = userRoleRepository;
            UserTokenRepository = userTokenRepository;
            UserClaimRepository = userClaimRepository;
            RoleClaimRepository = roleClaimRepository;
            LookupNormalizer = lookupNormalizer;
            Logger = logger;
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        // public IdentityUserStore(IRepository<TUser, TKey> userRepository, IRepository<TUserLogin, TKey> userLoginRepository, IRepository<TRole, TKey> roleRepository,
        //     IRepository<TUserRole, TKey> userRoleRepository, IRepository<TUserToken, TKey> userTokenRepository, IRepository<TUserClaim, TKey> userClaimRepository,
        //     IRepository<TRoleClaim, TKey> roleClaimRepository, ILookupNormalizer lookupNormalizer,
        //     ILogger<IdentityUserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserToken, TUserClaim, TRoleClaim>> logger, IdentityErrorDescriber describer = null)
        // {
        //     UserRepository = userRepository;
        //     UserLoginRepository = userLoginRepository;
        //     RoleRepository = roleRepository;
        //     UserRoleRepository = userRoleRepository;
        //     UserTokenRepository = userTokenRepository;
        //     UserClaimRepository = userClaimRepository;
        //     RoleClaimRepository = roleClaimRepository;
        //     LookupNormalizer = lookupNormalizer;
        //     Logger = logger;
        //     ErrorDescriber = describer ?? new IdentityErrorDescriber();
        // }
        //
        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";
        protected virtual IRepository<TUser, TUserKey> UserRepository { get; }

        protected virtual IRepository<TUserLogin, TUserLoginKey> UserLoginRepository { get; }

        protected virtual IRepository<TRole, TRoleKey> RoleRepository { get; }

        protected virtual IRepository<TUserRole> UserRoleRepository { get; }

        protected virtual IRepository<TUserToken, TUserTokenKey> UserTokenRepository { get; }

        protected virtual IRepository<TUserClaim, TUserClaimKey> UserClaimRepository { get; }

        protected virtual IRepository<TRoleClaim, TRoleClaimKey> RoleClaimRepository { get; }

        protected ILogger<IdentityUserStore<TUser, TUserKey, TRole, TRoleKey, TUserRole, TUserLogin, TUserLoginKey, TUserToken, TUserTokenKey, TUserClaim,
            TUserClaimKey, TRoleClaim, TRoleClaimKey>> Logger { get; set; }

        protected ILookupNormalizer LookupNormalizer { get; }
        public bool AutoSaveChanges { get; set; }

        public IdentityErrorDescriber ErrorDescriber { get; set; }


        public virtual void Dispose()
        {
        }

        public virtual Task<string> GetUserIdAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public virtual Task<string> GetUserNameAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.UserName);
        }

        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public virtual Task<string> GetNormalizedUserNameAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public virtual Task SetNormalizedUserNameAsync([NotNull] TUser user, [NotNull] string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public virtual async Task<IdentityResult> CreateAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.AddAsync(user, AutoSaveChanges, cancellationToken);

            return IdentityResult.Success;
        }


        public virtual async Task<IdentityResult> UpdateAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            try
            {
                await UserRepository.UpdateAsync(user, AutoSaveChanges, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            try
            {
                await UserRepository.DeleteAsync(user, AutoSaveChanges, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public virtual Task<TUser> FindByIdAsync([NotNull] string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var changeType = (TUserKey)Convert.ChangeType(userId, typeof(TUserKey));
            return UserRepository.FindAsync(changeType, cancellationToken: cancellationToken);
        }

        public virtual Task<TUser> FindByNameAsync([NotNull] string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(normalizedUserName, nameof(normalizedUserName));

            return Task.FromResult(UserRepository.FirstOrDefault(x => x.NormalizedUserName == normalizedUserName));
        }

        public virtual async Task AddLoginAsync([NotNull] TUser user, [NotNull] UserLoginInfo login, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(login, nameof(login));

            // var userResult = await UserRepository.GetAsync(user.Id, cancellationToken, x => x.Logins);
            //
            // userResult.AddLogin(login);
            await UserLoginRepository.AddAsync(new TUserLogin()
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey
            }, AutoSaveChanges, cancellationToken);
        }

        public virtual async Task RemoveLoginAsync([NotNull] TUser user, [NotNull] string loginProvider, [NotNull] string providerKey,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            var userLogin = new TUserLogin()
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            };
            await UserLoginRepository.DeleteAsync(userLogin, AutoSaveChanges, cancellationToken);
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            // user.Logins.Select(l =>
            //    new UserLoginInfo(
            //        l.LoginProvider,
            //        l.ProviderKey, l.LoginProvider)).ToList();
            Check.NotNull(user, nameof(user));
            // var func = EntityHelper.EntityEquality<TUserLogin, TKey>(user.Id);
            var userLoginInfos = UserLoginRepository.Where(x => x.UserId.Equals(user.Id)).Select(l =>
                new UserLoginInfo(
                    l.LoginProvider,
                    l.ProviderKey, l.LoginProvider)).ToList();
            return Task.FromResult<IList<UserLoginInfo>>(userLoginInfos);
        }

        public virtual async Task<TUser> FindByLoginAsync([NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default)
        {
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            var userLogin = await UserLoginRepository.SingleOrDefaultAsync(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey, cancellationToken);
            if (userLogin != null)
            {
                return await UserRepository.SingleOrDefaultAsync(userLogin.UserId, cancellationToken);
            }

            return null;
        }

        public virtual async Task AddToRoleAsync([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            Check.NotNull(user, nameof(user));
            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));

            if (await IsInRoleAsync(user, normalizedRoleName, cancellationToken))
            {
                return;
            }

            var role = RoleRepository.FirstOrDefault(x => x.NormalizedName == normalizedRoleName);
            if (role == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist!", normalizedRoleName));
            }

            await RoleRepository.AddAsync(role, AutoSaveChanges, cancellationToken: cancellationToken);
        }


        public virtual async Task RemoveFromRoleAsync([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName) + " can not be null or whitespace");
            }


            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, roleEntity.Id, cancellationToken);
                if (userRole != null)
                {
                    await UserRoleRepository.DeleteAsync(userRole, AutoSaveChanges, cancellationToken);
                }
            }
        }

        public virtual async Task<IList<string>> GetRolesAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));
            var identityUserRoles = UserRoleRepository.Where(x => x.UserId.Equals(user.Id));
            var roles = await RoleRepository.GetAllAsync(cancellationToken);
            var query = from userRole in identityUserRoles
                join role in roles on userRole.RoleId equals role.Id
                where userRole.UserId.Equals(user.Id)
                select role.Name;
            return await Task.FromResult<IList<string>>(query.ToList());
        }

        public virtual async Task<bool> IsInRoleAsync([NotNull] TUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName) + " can not be null or whitespace");
            }

            var roles = await GetRolesAsync(user, cancellationToken);

            return roles
                .Select(r => LookupNormalizer.NormalizeName(r))
                .Contains(normalizedRoleName);
        }

        public virtual async Task<IList<TUser>> GetUsersInRoleAsync([NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var query = from userRole in UserRoleRepository.GetAll()
                    join user in UserRepository.GetAll() on userRole.UserId equals user.Id
                    where userRole.RoleId.Equals(role.Id)
                    select user;

                return await Task.FromResult<IList<TUser>>(query.ToList());
            }

            return new List<TUser>();
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            Check.NotNull(user, nameof(user));

            var claims = await UserClaimRepository.FindAsync(x => x.UserId.Equals(user.Id), cancellationToken);
            return claims.Select(c => c.ToClaim()).ToList();
        }

        public virtual async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            var userClaims = new List<TUserClaim>();
            foreach (var claim in claims)
            {
                userClaims.Add(new TUserClaim()
                {
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }

            await UserClaimRepository.AddManyAsync(userClaims, cancellationToken: cancellationToken);
        }

        public virtual async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claim, nameof(claim));
            Check.NotNull(newClaim, nameof(newClaim));

            var claims = await UserClaimRepository.GetAllMergeAsync(x => x.UserId.Equals(user.Id) && x.ClaimValue == claim.Value && x.ClaimType == claim.Type,
                cancellationToken);

            foreach (var userClaim in claims)
            {
                userClaim.SetClaim(newClaim);
            }
        }

        public virtual async Task RemoveClaimsAsync([NotNull] TUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            // foreach (var claim in claims)
            // {
            //     var matchedClaims = await UserClaimRepository.GetAllMergeAsync(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type,
            //         cancellationToken);
            //     foreach (var c in matchedClaims)
            //     {
            //         await UserClaimRepository.DeleteAsync(c, cancellationToken: cancellationToken);
            //     }
            // }
            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            await UserClaimRepository.DeleteManyAsync(uc => uc.UserId.Equals(user.Id) && claims.Select(x => x.Value).Contains(uc.ClaimValue) && claims.Select(x => x.Type).Contains
                (uc.ClaimType), cancellationToken: cancellationToken);
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task SetPasswordHashAsync([NotNull] TUser user, string passwordHash, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetPasswordHashAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PasswordHash != null);
        }

        public virtual Task SetSecurityStampAsync([NotNull] TUser user, string stamp, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetSecurityStampAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        public virtual Task SetEmailAsync([NotNull] TUser user, string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(email, nameof(email));

            user.Email = email;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetEmailAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email);
        }

        public virtual Task<bool> GetEmailConfirmedAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public virtual Task SetEmailConfirmedAsync([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return UserRepository.SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        public virtual Task SetNormalizedEmailAsync([NotNull] TUser user, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public virtual Task SetLockoutEndDateAsync([NotNull] TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.LockoutEnd = lockoutEnd;

            return Task.CompletedTask;
        }

        public virtual Task<int> IncrementAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task ResetAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public virtual Task<int> GetAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task<bool> GetLockoutEnabledAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public virtual Task SetLockoutEnabledAsync([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        public virtual Task SetPhoneNumberAsync([NotNull] TUser user, string phoneNumber, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public virtual Task<string> GetPhoneNumberAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual Task SetPhoneNumberConfirmedAsync([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public virtual Task SetTwoFactorEnabledAsync([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public virtual async Task SetTokenAsync([NotNull] TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Check.NotNull(user, nameof(user));

            var token = await UserTokenRepository.SingleOrDefaultAsync(t => t.LoginProvider == loginProvider && t.Name == name, cancellationToken);
            if (token == null)
            {
                var userToken = new TUserToken { LoginProvider = loginProvider, Name = name, Value = value, UserId = user.Id };

                await UserTokenRepository.AddAsync(userToken, AutoSaveChanges, cancellationToken);
            }
            else
            {
                token.Value = value;
            }
        }

        public virtual async Task RemoveTokenAsync([NotNull] TUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserTokenRepository.DeleteAsync(x => x.UserId.Equals(user.Id) && x.LoginProvider == loginProvider && x.Name == name, AutoSaveChanges, cancellationToken);
        }

        public virtual async Task<string> GetTokenAsync([NotNull] TUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return (await UserTokenRepository.SingleOrDefaultAsync(x => x.UserId.Equals(user.Id) && x.LoginProvider == loginProvider && x.Name == name, cancellationToken))?.Value;
        }

        public virtual async Task SetAuthenticatorKeyAsync([NotNull] TUser user, [NotNull] string key, CancellationToken cancellationToken)
        {
            await SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public virtual Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken = default)
        {
            return GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        public virtual Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken = default)
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public virtual async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(code, nameof(code));

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }

            return false;
        }

        public virtual async Task<int> CountCodesAsync([NotNull] TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }

            return 0;
        }


        protected virtual Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            return RoleRepository.SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
        }

        protected virtual Task<TUserRole> FindUserRoleAsync(TUserKey userId, TRoleKey roleId, CancellationToken cancellationToken = default)
        {
            return UserRoleRepository.SingleOrDefaultAsync(x => x.UserId.Equals(userId) && x.RoleId.Equals(roleId), cancellationToken);
        }
    }
}