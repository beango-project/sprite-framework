using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public abstract class Entity<TKey> : Entity, IEntity<TKey> where TKey : IEquatable<TKey>
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
        [Key]
        public virtual TKey Id { get; protected set; }

        public override object[] GetKeys()
        {
            return new object[] { Id };
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

        // public int CompareTo(TKey? other)
        // {
        //     var hashCode = other?.GetHashCode() ^ 31;
        //
        //     if (!_requestedHashCode.HasValue && hashCode.HasValue)
        //     {
        //         return _requestedHashCode == hashCode.Value ? 0 : 1;
        //     }
        //
        //     // var arg = FastExpressionCompiler.LightExpression.Expression.Parameter(typeof(Entity<TKey>));
        //     // var equals = FastExpressionCompiler.LightExpression.Expression.Equal(Expression.Property(arg, "Id"), Expression.Constant(other))
        //     // var expression = FastExpressionCompiler.LightExpression.Expression.Lambda<Func<TKey, bool>>(@equals).ToExpression() x;
        //     return 1;
        // }

        public bool Equals(TKey? other)
        {
            return this.Equals(obj: other);
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

            var item = (Entity<TKey>)obj;

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