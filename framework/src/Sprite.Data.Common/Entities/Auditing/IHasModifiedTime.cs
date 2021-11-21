using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IHasModifiedTime
    {
        [LastModifiedDate]
        DateTime ModifiedTime { get; }
    }
}