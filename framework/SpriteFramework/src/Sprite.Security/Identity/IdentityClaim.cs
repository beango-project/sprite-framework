using System;
using System.Security.Claims;
using JetBrains.Annotations;
using Sprite.Data.Entities;

namespace Sprite.Security.Identity
{
    public abstract class IdentityClaim<TKey> : Entity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public virtual string ClaimType { get; protected internal set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public virtual string ClaimValue { get; protected internal set; }

        protected IdentityClaim()
        {
        }

        protected internal IdentityClaim(TKey id, [NotNull] Claim claim)
            : this(id, claim.Type, claim.Value)
        {
        }

        protected internal IdentityClaim(TKey id, [NotNull] string claimType, string claimValue)
        {
            Check.NotNull(claimType, nameof(claimType));

            Id = id;
            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        /// <summary>
        /// Creates a Claim instance from this entity.
        /// </summary>
        /// <returns></returns>
        public virtual Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }

        public virtual void SetClaim([NotNull] Claim claim)
        {
            Check.NotNull(claim, nameof(claim));

            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}