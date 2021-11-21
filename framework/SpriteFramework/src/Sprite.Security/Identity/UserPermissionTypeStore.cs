using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Entities;
using Sprite.Data.Repositories;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;
using Sprite.Security.Permissions;

namespace Sprite.Security.Identity
{
    public class UserPermissionTypeStore<TUserClaim, TUserClaimKey, TUserKey> : IPermissionStore
        where TUserClaim : IdentityUserClaim<TUserClaimKey, TUserKey>, new() where TUserClaimKey : IEquatable<TUserClaimKey>
    {
        private readonly IRepository<TUserClaim, TUserClaimKey> _identityUserClaimRepository;

        public UserPermissionTypeStore(IRepository<TUserClaim, TUserClaimKey> identityUserClaimRepository)
        {
            _identityUserClaimRepository = identityUserClaimRepository;
        }


        public Task<IQueryable<Permission>> GetPermissionsAsync(Func<Permission, bool> filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        [Transactional(Propagation.Supports)]
        public async Task<bool> IsGrantedAsync(string name, string ClaimValue, CancellationToken cancellationToken = default)
        {
            var userKey = Convert.ChangeType(ClaimValue, typeof(TUserKey));
            var identityUserClaim = await _identityUserClaimRepository.GetAsync(x => x.UserId.Equals(userKey) && x.ClaimValue == name && x.ClaimType == "User", cancellationToken);
            return await Task.FromResult(identityUserClaim != null);
        }

        [Transactional(Propagation.Supports)]
        public async Task SetPermissionsAsync(string permissionName, string providerKey, string ClaimValue, CancellationToken cancellationToken = default)
        {
            var userClaim = new TUserClaim()
            {
                UserId = (TUserKey)Convert.ChangeType(ClaimValue, typeof(TUserKey)),
                ClaimType = providerKey,
                ClaimValue = permissionName
            };
            await _identityUserClaimRepository.AddAsync(userClaim, cancellationToken: cancellationToken);
        }
    }
}