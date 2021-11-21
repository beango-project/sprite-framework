using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface ICreationAndModifiedEntity<TKey, TUserKey> : ICreationAuditedEntity<TKey, TUserKey>, IHasModifiedTime
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// 上次修改者Id
        /// </summary>
        TUserKey LastModifierId { get; }
    }

    public interface ICreationAndModifiedEntity<TKey, TUser, TUserKey> : ICreationAuditedEntity<TKey, TUser, TUserKey>, ICreationAndModifiedEntity<TKey, TUserKey>
        where TKey : IEquatable<TKey>
        where TUser : IEntity<TUserKey>
        where TUserKey : IEquatable<TUserKey>

    {
        /// <summary>
        /// 上次修改者
        /// </summary>
        TUser LastModifier { get; }
    }
}