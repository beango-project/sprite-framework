using System;
using Sprite.Data.Entities;

namespace Sprite.Security.Identity
{
    public class IdentityUserToken<TKey,TUserKey> : Entity<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TUserKey UserId { get; protected internal set; }

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public virtual string Value { get; set; }

        public IdentityUserToken()
        {
        }

        public IdentityUserToken(TUserKey userId, string loginProvider, string name, string value)
        {
            UserId = userId;
            LoginProvider = loginProvider;
            Name = name;
            Value = value;
        }


    }
}