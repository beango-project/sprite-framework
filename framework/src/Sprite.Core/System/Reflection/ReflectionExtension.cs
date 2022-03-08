using System;
using System.ComponentModel;
using ImmediateReflection;

#nullable enable
namespace System.Reflection
{
    public static class ReflectionExtension
    {
        /// <summary>
        ///     判断类型是否为Nullable类型
        /// </summary>
        /// <param name="type"> 要处理的类型 </param>
        /// <returns> 是返回True，不是返回False </returns>
        public static bool IsNullableType(this Type type)
        {
            return (type != null) && type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        /// <summary>
        ///     通过类型转换器获取Nullable类型的基础类型
        /// </summary>
        /// <param name="type"> 要处理的类型对象 </param>
        /// <returns> </returns>
        public static Type GetUnNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                var nullableConverter = new NullableConverter(type);
                return nullableConverter.UnderlyingType;
            }

            return type;
        }


        // [CanBeNull]
        // public static TAttribute? GetAttributeWithDefined<TAttribute>(this Type type, bool inherit = true)
        //     where TAttribute : Attribute
        //     => GetAttributeWithDefined<TAttribute>(type.mem, inherit);


        public static bool IsDefined<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
        {
            return memberInfo.IsDefinedImmediateAttribute(typeof(TAttribute), inherit);
        }

        public static bool IsDefinedAttribute(this MemberInfo memberInfo, Type attributeType, bool inherit = false)
        {
            return memberInfo.IsDefinedImmediateAttribute(attributeType, inherit);
        }

        public static TAttribute? GetAttributeWithDefined<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
            where TAttribute : Attribute
        {
            if (memberInfo.IsDefinedImmediateAttribute(typeof(TAttribute), inherit))
            {
                return memberInfo.GetImmediateAttribute<TAttribute>(inherit);
            }

            return null;
        }
    }
}