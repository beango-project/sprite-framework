using System;
using System.Collections.Generic;
using ImTools;
using JetBrains.Annotations;

namespace Sprite.DependencyInjection
{
    public class SwapSpace : ISwapSpace
    {
        public SwapSpace()
        {
            _pool = ImHashMap<Type, object>.Empty;
        }

        private ImHashMap<Type, object> _pool;

        [CanBeNull]
        public object GetSet(Type key, object value)
        {
            if (!_pool.Contains(key))
            {
                throw new KeyNotFoundException($"Cannot get the key of {nameof(key)}");
            }

            //
            // var tValue = Pool[key];
            // Pool[key] = value;
            _pool.TryFind(key, out var newValue);
            _pool = _pool.Update(key, value);
            return newValue;
        }

        public T GetSet<T>(object value)
        {
            return (T)GetSet(typeof(T), value);
        }


        public object Get(Type key)
        {
            return _pool.GetValueOrDefault(key);
        }

        public object TryGet(Type key)
        {
            _pool.TryFind(key, out var value);
            return value;
        }

        public T TryGet<T>()
        {
            return (T)TryGet(typeof(T));
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// 尝试将指定的键和值添加到交换区
        /// Attempts to add the specified key and value to the SwapSpace
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(Type key, object value = null)
        {
            _pool = _pool.AddOrUpdate(key, value);
        }

        public void Add<T>(object value = null)
        {
            Add(typeof(T), value);
        }

        public bool TryAdd(Type key, object value = null)
        {
            var isContains = _pool.Contains(key);
            if (!isContains)
            {
                _pool = _pool.AddOrUpdate(key, value);
            }

            return isContains;
        }

        public bool TryAdd<T>(object value = null)
        {
            return TryAdd(typeof(T), value);
        }

        public void Set(Type key, object value)
        {
            if (!_pool.Contains(key))
            {
                throw new KeyNotFoundException($"Cannot get the key of {nameof(key)}");
            }

            _pool = _pool.Update(key, value);
        }

        public void Set<T>(object value)
        {
            Set(typeof(T), value);
        }
    }
}