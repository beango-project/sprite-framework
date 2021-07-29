using System;

namespace Sprite.Data.Entities.Auditing
{
    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        /// Deletion time.
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }
}