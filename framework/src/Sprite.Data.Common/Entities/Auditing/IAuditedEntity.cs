using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IAuditedEntity<TKey, TUserKey> : ICreationAndModifiedEntity<TKey, TUserKey>, IDeletionAuditedEntity<TKey, TUserKey>
        where TKey : IEquatable<TKey> where TUserKey : IEquatable<TUserKey>
    {
    }

    // public interface IAuditedEntity<TKey,TUser> : ICreationAndModifiedEntity<TKey,TUser>, IDeletionAuditedEntity<TUser, TKey>
    //     where TKey : IEquatable<TKey>
    //     where TUser : IEntity<TKey>
    // {
    // }
}