using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace System
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        [CanBeNull]
        public static T CastTo<T>(this object value)
        {
            if (value == null && default(T) == null)
            {
                return default(T);
            }

            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }

            object result = CastTo(value, typeof(T));
            return (T)result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                return CastTo<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }


        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        public static bool IsIn<T>(this T item, IEnumerable<T> items)
        {
            return items != null && items.Contains(item);
        }
    }
}