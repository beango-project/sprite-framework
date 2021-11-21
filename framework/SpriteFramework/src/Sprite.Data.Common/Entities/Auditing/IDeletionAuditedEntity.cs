using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IDeletionAuditedEntity<TKey> : IHasDeletionTime where TKey : IEquatable<TKey>
    {
        TKey DeleterId { get; set; }
    }

    public interface IDeletionAuditedEntity<TUser, TKey> : IDeletionAuditedEntity<TKey> where TKey : IEquatable<TKey>
    {
        TUser Deleter { get; set; }
    }
}