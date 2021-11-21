using System;
using Sprite.Data.Entities;
using Sprite.Data.Entities.Auditing;

namespace Sprite.Security.Identity
{
    public class IdentityUserRole<TUserKey, TRoleKey> : Entity
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        /// <summary>
        /// Gets or sets the primary key of the user that is linked to a role.
        /// </summary>
        public virtual TUserKey UserId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the role that is linked to the user.
        /// </summary>
        public virtual TRoleKey RoleId { get; set; }

        // [PR]
        // public virtual IdentityUser<TUserKey> User { get; set; }
        //
        // public virtual IdentityRole<TRoleKey> Role { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { UserId, RoleId };
        }
    }
}