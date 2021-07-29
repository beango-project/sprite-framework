using System;

namespace Sprite.Data.Entities
{
    /// <inheritdoc />
    [Serializable]
    public abstract class Entity : IEntity
    {
        public abstract object[] GetKeys();

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] Keys = {string.Join(",", GetKeys())}";
        }
    }

    /// <inheritdoc cref="IEntity{TKey}" />
    [Serializable]
    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        private int? _requestedHashCode;

        protected Entity()
        {
        }

        protected Entity(TKey id)
        {
            Id = id;
        }

        /// <inheritdoc />
        public virtual TKey Id { get; protected set; }

        public override object[] GetKeys()
        {
            return new object[] {Id};
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] Id = {Id}";
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                {
                    _requestedHashCode = Id.GetHashCode() ^ 31;
                }

                return _requestedHashCode.Value;
            }

            return base.GetHashCode();
        }

        private bool IsTransient()
        {
            return EntityHelper.HasDefaultId(this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TKey>))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var item = (Entity<TKey>) obj;

            if (item.IsTransient() || IsTransient())
            {
                return false;
            }

            return item.Id.Equals(Id);
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null) ? true : false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}