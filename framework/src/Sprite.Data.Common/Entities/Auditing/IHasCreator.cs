using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprite.Data.Entities.Auditing
{
    public interface IHasCreator<TCreator, TKey> : IHasCreator<TKey>
        where TCreator : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        [CreateBy]
        TKey CreatorId { get; }

        [ForeignKey("CreatorId")]
        TCreator Creator { get; }
    }

    public interface IHasCreator<TKey>
        where TKey : IEquatable<TKey>
    {
        [CreateBy]
        TKey CreatorId { get; }
    }
}