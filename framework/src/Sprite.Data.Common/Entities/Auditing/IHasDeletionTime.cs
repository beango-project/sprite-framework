using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        /// Deletion time.
        /// </summary>
        [DeletionTime]
        DateTime? DeletionTime { get; set; }
    }
}