using System;
using System.Diagnostics.Metrics;
using JetBrains.Annotations;

namespace Sprite.DependencyInjection
{
#nullable enable
    /// <summary>
    /// 交换区,用于解决前期注入时的循环依赖.
    /// 警告！只建议添加 Singleton 的对象,添加其他生命周期的对象容易造成内存泄漏
    /// </summary>
    public interface ISwapSpace
    {
        bool IsEmpty { get; }

        int Count();

        /// <summary>
        /// 根据key获取其的旧值，并设置一个新值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [CanBeNull]
        object GetSet(Type key, object value);

        [CanBeNull]
        T GetSet<T>(object value);

        [CanBeNull]
        object Get(Type key);

        bool TryGet(Type key, out object value);


        bool TryGet<T>(out T? value);

        [CanBeNull]
        T Get<T>();

        void Add(Type key, object value = null);

        void Add<T>(object value = null);

        bool TryAdd(Type key, object value = null);

        bool TryAdd<T>(object value = null);

        void Set(Type key, object value);

        void Set<T>(object value);

        void Remove(Type key);

        void Remove<T>();

        void Clear();
    }
}