using System;

namespace Sprite.Data.Entities.Auditing
{
    public abstract class CreationAndModifiedEntity<TKey, TUserKey> : CreationAuditedEntity<TKey, TUserKey>, ICreationAndModifiedEntity<TKey, TUserKey>
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
    {
        [LastModifiedDate]
        public virtual DateTime ModifiedTime { get; protected set; }

        [LastModifiedBy]
        public virtual TUserKey LastModifierId { get; protected set; }
    }

    public abstract class CreationAndModifiedEntity<TKey, TUser, TUserKey> : CreationAndModifiedEntity<TKey, TUserKey>, ICreationAndModifiedEntity<TKey, TUser, TUserKey>
        where TKey : IEquatable<TKey>
        where TUser : IEntity<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public virtual TUser Creator { get; protected set; }
        
        public virtual TUser LastModifier { get; protected set; }
    }
}