using System;

namespace Sprite.Data.Entities.Auditing
{
    public abstract class CreationAudited<TCreator, TKey> : ICreationAudited<TCreator, TKey>
        where TCreator : IEntity<TKey>
    {
        public virtual TCreator CreateUser { get; }
        public virtual DateTime CreationTime { get; }
    }
}