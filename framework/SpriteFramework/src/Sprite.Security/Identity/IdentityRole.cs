using System;
using System.Collections.Generic;
using Sprite.Data.Entities;
using Sprite.Data.Entities.Auditing;

namespace Sprite.Security.Identity
{
    public abstract class IdentityRole<TKey> : Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }

        public void SetNormalizedName()
        {
            NormalizedName = Name.ToUpperInvariant();
        }
    }
}