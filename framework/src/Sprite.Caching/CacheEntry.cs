using System;

namespace Sprite.Caching
{
    public abstract class CacheEntry : IDisposable
    {
        private TimeSpan? _slidingExpiration;
        internal DateTimeOffset? _absoluteExpiration;
        internal TimeSpan? _absoluteExpirationRelativeToNow;
        private bool _isExpired;
        
        /// <summary>
        /// 删除原因
        /// </summary>
        public ReasonDeletion ReasonDeletion { get; set; }
        
        /// <summary>
        /// 最后访问时间
        /// </summary>
        internal DateTimeOffset LastAccessed { get; set; }

        /// <summary>
        /// 绝对过期时间
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration
        {
            get => _absoluteExpiration;
            set => _absoluteExpiration = value;
        }

        /// <summary>
        /// 相对于现在的绝对过期时间
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get => _absoluteExpirationRelativeToNow;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(AbsoluteExpirationRelativeToNow),
                        value,
                        "The relative expiration value must be positive.");
                }

                _absoluteExpirationRelativeToNow = value;
            }
        }

        /// <summary>
        /// 已过期
        /// </summary>
        public bool IsExpired => CheckExpired(DateTimeOffset.Now);

        internal bool CheckExpired(DateTimeOffset now)
        {
            return _isExpired || CheckForExpiredTime(now);
        }

        private bool CheckForExpiredTime(DateTimeOffset now)
        {
            if (_absoluteExpiration.HasValue && _absoluteExpiration.Value <= now)
            {
                SetExpired(ReasonDeletion.Expired);
                return true;
            }

            if (_slidingExpiration.HasValue
                && (now - LastAccessed) >= _slidingExpiration)
            {
                SetExpired(ReasonDeletion.Expired);
                return true;
            }

            return false;
        }

        internal void SetExpired(ReasonDeletion reason)
        {
            if (ReasonDeletion == ReasonDeletion.None)
            {
                ReasonDeletion = reason;
            }

            _isExpired = true;
        }

        /// <summary>
        /// 滑动过期
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan? SlidingExpiration
        {
            get => _slidingExpiration;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(SlidingExpiration),
                        value,
                        "The sliding expiration value must be positive.");
                }

                _slidingExpiration = value;
            }
        }

        public abstract void Dispose();
    }

    public class CacheEntry<T> : CacheEntry, IEquatable<CacheEntry<T?>?>
    {
        private T? _value;
        private bool _valueIsSet = false;
        private bool _disposed;


        // private IDisposable _scope;
        public CacheEntry(string key, T? value, TimeSpan timeToLive) : this(key, value, DateTimeOffset.UtcNow + timeToLive)
        {
        }

        public CacheEntry(string key, T? value, DateTimeOffset expiry)
        {
            Value = value;
        }


        protected CacheEntry(DateTimeOffset expiry)
        {
            AbsoluteExpiration = expiry;
        }


        public string Key { get; }

        /// <summary>
        /// 缓存的值。
        /// </summary>
        public T? Value
        {
            get => _value;
            set
            {
                _value = value;
                _valueIsSet = true;
            }
        }

        /// <inheritdoc />
        public bool Equals(CacheEntry<T?>? other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Value, other.Value) &&
                   AbsoluteExpiration == other.AbsoluteExpiration;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is CacheEntry<T?> objOfType)
            {
                return Equals(objOfType);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Value?.GetHashCode() ?? 1) ^ AbsoluteExpiration.GetHashCode();
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}