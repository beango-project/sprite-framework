using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface ICreationAuditedEntity<TKey, TUserKey> : IEntity<TKey>, IHasCreator<TUserKey>, IHasCreationTime
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
    {
    }

    public interface ICreationAuditedEntity<TKey, TUser, TUserKey> : IEntity<TKey>, IHasCreator<TUser, TUserKey>, IHasCreationTime
        where TKey : IEquatable<TKey>
        where TUser : IEntity<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
    }
}