using System;
using Sprite.Data.Entities;
using Sprite.Data.Entities.Auditing;

namespace Sprite.Security.Identity
{
    public class IdentityRoleClaim<TKey,TRoleKey> : IdentityClaim<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TRoleKey RoleId { get; protected internal set; }
    }
}