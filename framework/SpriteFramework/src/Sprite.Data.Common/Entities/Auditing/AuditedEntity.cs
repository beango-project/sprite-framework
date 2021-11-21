using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprite.Data.Entities.Auditing
{
    // public abstract class AuditedEntity<TKey> : CreationAndModifiedEntity<TKey>, IAuditedEntity<TKey>
    //     where TKey : IEquatable<TKey>
    // {
    //     public virtual bool IsDeleted { get; set; }
    //     public virtual DateTime? DeletionTime { get; set; }
    //     public virtual TKey DeleterId { get; set; }
    // }
    //
    // public abstract class AuditedEntity<TUser, TKey> : CreationAndModifiedEntity<TUser, TKey>, IAuditedEntity<TUser, TKey>
    //     where TKey : IEquatable<TKey>
    //     where TUser : IEntity<TKey>
    // {
    //     public virtual bool IsDeleted { get; set; }
    //     public virtual DateTime? DeletionTime { get; set; }
    //     public virtual TKey DeleterId { get; set; }
    //     public virtual TUser Deleter { get; set; }
    // }
}