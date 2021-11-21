using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IHasCreationTime
    {
        [CreatedDate]
        DateTime CreationTime { get; }
    }
}