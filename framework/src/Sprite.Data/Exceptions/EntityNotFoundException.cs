﻿using System;

namespace Sprite.Data.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Type of the entity.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Id of the Entity.
        /// </summary>
        public object Id { get; set; }


        public EntityNotFoundException(Type entityType, object id)
        {
            EntityType = entityType;
            Id = id;
        }
    }
}