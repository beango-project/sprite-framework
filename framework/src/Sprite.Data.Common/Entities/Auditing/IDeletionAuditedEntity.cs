using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IDeletionAuditedEntity<TKey, TUserKey> : IEntity<TKey>, IHasDeletionTime
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
    {
        TUserKey DeleterId { get; set; }
    }

    public interface IDeletionAuditedEntity<TKey, TUserKey, TUser> : IDeletionAuditedEntity<TKey, TUserKey>
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
    {
        TUser Deleter { get; set; }
    }
}