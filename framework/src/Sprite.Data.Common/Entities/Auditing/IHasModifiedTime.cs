using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IHasModifiedTime
    {
        DateTime? ModifiedTime { get; }
    }
}