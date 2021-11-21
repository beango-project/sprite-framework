using System;

namespace Sprite.Data.Entities.Auditing
{
    public abstract class CreationAuditedEntity<TKey, TUserKey> : Entity<TKey>, ICreationAuditedEntity<TKey, TUserKey>
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
    {
        [CreateBy]
        public virtual TUserKey CreatorId { get; protected set; }

        [CreatedDate]
        public virtual DateTime CreationTime { get; protected set; }
    }

    public abstract class CreationAuditedEntity<TKey, TUser, TUserKey> : CreationAuditedEntity<TKey, TUserKey>, ICreationAuditedEntity<TKey,TUser,TUserKey >
        where TKey : IEquatable<TKey>
        where TUser : IEntity<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public TUser Creator { get; }
    }
}