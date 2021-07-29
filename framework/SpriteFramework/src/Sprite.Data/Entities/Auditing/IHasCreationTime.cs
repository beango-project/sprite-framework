using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IHasCreationTime
    {
        DateTime CreationTime { get; }
    }
}