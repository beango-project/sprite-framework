using System;
using System.Security.Claims;
using JetBrains.Annotations;
using Sprite.Data.Entities;

namespace Sprite.Security.Identity
{
    public class IdentityUserClaim<TKey, TUserKey> : IdentityClaim<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TUserKey UserId { get; protected internal set; }

        public IdentityUserClaim()
        {
        }

        protected internal IdentityUserClaim(TKey id, TUserKey userId, [NotNull] Claim claim) : base(id, claim)
        {
            UserId = userId;
        }

        public IdentityUserClaim(TKey id, TUserKey userId, [NotNull] string claimType, string claimValue)
            : base(id, claimType, claimValue)
        {
            UserId = userId;
        }
    }
}