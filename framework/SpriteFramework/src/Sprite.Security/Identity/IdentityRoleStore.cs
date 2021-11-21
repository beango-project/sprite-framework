using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Sprite.Data.Repositories;

namespace Sprite.Security.Identity
{
    public class IdentityRoleStore<TUser, TUserKey, TRole, TRoleKey, TRoleClaim, TRoleClaimKey> :
        IRoleStore<TRole>,
        IRoleClaimStore<TRole>
        where TUser : IdentityUser<TUserKey>, new()
        where TRole : IdentityRole<TRoleKey>, new()
        where TRoleClaim : IdentityRoleClaim<TRoleClaimKey, TRoleKey>, new()
        where TRoleKey : IEquatable<TRoleKey>
        where TRoleClaimKey : IEquatable<TRoleClaimKey>
        where TUserKey : IEquatable<TUserKey>

    {
        public IdentityRoleStore(IRepository<TRole, TRoleKey> roleRepository, IRepository<TRoleClaim, TRoleClaimKey> roleClaimRepository, ILogger<IdentityRoleStore<TUser,
            TUserKey, TRole, TRoleKey, TRoleClaim, TRoleClaimKey>> logger, IdentityErrorDescriber describer = null)
        {
            RoleRepository = roleRepository;
            RoleClaimRepository = roleClaimRepository;
            Logger = logger;
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }


        protected IRepository<TRole, TRoleKey> RoleRepository { get; }

        protected IRepository<TRoleClaim, TRoleClaimKey> RoleClaimRepository { get; }

        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public bool AutoSaveChanges { get; set; } = true;

        public ILogger<IdentityRoleStore<TUser, TUserKey, TRole, TRoleKey, TRoleClaim, TRoleClaimKey>> Logger { get; }

        public virtual void Dispose()
        {
        }

        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            Check.NotNull(role, nameof(role));
            await RoleRepository.AddAsync(role, AutoSaveChanges, cancellationToken);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync([NotNull] TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            try
            {
                await RoleRepository.UpdateAsync(role, AutoSaveChanges, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync([NotNull] TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            try
            {
                await RoleRepository.DeleteAsync(role, AutoSaveChanges, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public virtual Task<string> GetRoleIdAsync([NotNull] TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.Id.ToString());
        }

        public virtual Task<string> GetRoleNameAsync([NotNull] TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.Name);
        }

        public virtual Task SetRoleNameAsync([NotNull] TRole role, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        public virtual Task<string> GetNormalizedRoleNameAsync([NotNull] TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public virtual Task SetNormalizedRoleNameAsync([NotNull] TRole role, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public virtual Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (roleId.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            var id = (TRoleKey) Convert.ChangeType(roleId, typeof(TRoleKey));
            return RoleRepository.SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        }

        public virtual Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));
            if (normalizedRoleName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            return RoleRepository.SingleOrDefaultAsync(x => x.NormalizedName.Equals(normalizedRoleName), cancellationToken);
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));

            var identityRoleClaims = await RoleClaimRepository.GetAllMergeAsync(x => x.RoleId.Equals(role.Id), cancellationToken);

            return identityRoleClaims.Select(c => c.ToClaim()).ToList();
        }

        public virtual async Task AddClaimAsync([NotNull] TRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));


            await RoleClaimRepository.AddAsync(CreateRoleClaim(role, claim), AutoSaveChanges, cancellationToken: cancellationToken);
        }

        protected virtual TRoleClaim CreateRoleClaim(TRole role, Claim claim)
            => new TRoleClaim {RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value};

        public virtual async Task RemoveClaimAsync([NotNull] TRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));

            await RoleClaimRepository.DeleteAsync(x => x.RoleId.Equals(role.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value, AutoSaveChanges,
                cancellationToken: cancellationToken);
        }
    }
}