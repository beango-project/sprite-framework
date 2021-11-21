using System;

namespace Sprite.DependencyInjection
{
    /// <summary>
    /// 交换区
    /// </summary>
    public interface ISwapSpace
    {
        /// <summary>
        /// 根据key获取其的旧值，并设置一个新值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        object GetSet(Type key, object value);

        T GetSet<T>(object value);

        object Get(Type key);

        object TryGet(Type key);

        T TryGet<T>();

        T Get<T>();

        void Add(Type key, object value = null);

        void Add<T>(object value = null);

        bool TryAdd(Type key, object value = null);

        bool TryAdd<T>(object value = null);

        void Set(Type key, object value);

        void Set<T>(object value);
    }
}