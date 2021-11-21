using System;
using Sprite.Data.Entities;

namespace Sprite.Security.Identity
{
    public abstract class IdentityUserLogin<TKey, TUserKey> : Entity<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TUserKey UserId { get; set; }

        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }

        public IdentityUserLogin()
        {
        }

        public IdentityUserLogin(TUserKey userId, string loginProvider, string providerKey)
        {
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }
    }
}