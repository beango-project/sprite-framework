using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace System.Reflection
{
    public class TypeHelper
    {
        /// <summary>
        /// 是为原始类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive;
        }


        public static bool IsTypeOfDisplayLiteralExpression(Type type)
        {
            if (IsPrimitiveType(type))
            {
                return true;
            }

            if (type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid))
            {
                return true;
            }

            return false;
        }


        public static bool IsMatch(Type type, Type from)
        {
            if (type.IsClass && !type.IsAbstract && !type.IsGenericType)
            {
                var meetType = FromClassHierarchyFind(type, from);
                if (meetType != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static Type GetMatchType2(Type type, Type from, bool ignoreMismatchExceptions = false)
        {
            //确定类型是class，就从类层次结构中查找 from的
            if (type.IsClass && from.IsClass)
            {
                var meetType = FromClassHierarchyFind2(type, from);
                if (meetType != null)
                {
                    return meetType;
                }
            }

            if ((type.IsInterface || type.IsClass) && from.IsInterface)
            {
                var meetType = FromClassHierarchyFind2(type, from);
                if (meetType != null)
                {
                    return meetType;
                }
            }

            //接口不可能派生于类
            if (type.IsInterface && from.IsClass)
            {
                return null;
            }

            return null;
        }


        [CanBeNull]
        private static Type FromClassHierarchyFind2(Type type, Type from, bool isFindInterface = false)
        {
            //如果是接口，则对比接口，如果是类from是接口则获取接口获取
            isFindInterface = from.IsInterface;
            //先判断两个类型是否相等
            if (type == from)
            {
                return type;
            }

            //如果是泛型的话
            if (type.IsGenericType)
            {
                //判断泛型定义是否相等，相等则返回
                if (type.GetGenericTypeDefinition() == from)
                {
                    return type;
                }

                if (isFindInterface)
                {
                    return type.GetInterfaces().FirstOrDefault(x => x == from);
                }

                //不相等则获取基类来进行判断
                if (type.BaseType != null)
                {
                    return FromClassHierarchyFind2(type.BaseType, from);
                }

                return null;
            }

            //从类的基类查找
            if (type.BaseType != null && !isFindInterface)
            {
                return FromClassHierarchyFind2(type.BaseType, from);
            }

            if (isFindInterface)
            {
                return type.GetInterfaces().FirstOrDefault(x => x == from);
            }

            return null;
        }

        static Type FromClassHierarchyFindInterface(Type type, Type from)
        {
            //TODO:
            if (type.IsClass)
            {
                var findType = type.GetInterfaces().FirstOrDefault(x => x == from);
                if (findType != null)
                {
                    return findType;
                }


                if (type.BaseType != null)
                {
                    // FromClassHierarchyFindInterface()
                }
            }

            if (type.IsInterface)
            {
            }

            return null;
        }

        public static Type GetMatchType(Type type, Type from, bool ignoreMismatchExceptions = false)
        {
            if (type.IsClass && !type.IsAbstract && !type.IsGenericType)
            {
                var meetType = FromClassHierarchyFind(type, from);
                if (meetType != null)
                {
                    return meetType;
                }
            }

            if (ignoreMismatchExceptions)
            {
                throw new ArgumentException();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="from"></param>
        /// <param name="meetType"></param>
        /// <returns></returns>
        [CanBeNull]
        private static Type FromClassHierarchyFind(Type type, Type from)
        {
            // if type is class and is not abstract and is generic type 
            if (type.IsClass && !type.IsAbstract && !type.IsGenericType)
            {
                if (type == from)
                {
                    return type;
                }

                if (type.BaseType != null)
                {
                    return FromClassHierarchyFind(type.BaseType, from);
                }
            }
            else if (type.IsClass)
            {
                if (type.IsGenericType)
                {
                    if (type.GetGenericTypeDefinition() == from)
                    {
                        return type;
                    }

                    if (type.BaseType != null)
                    {
                        return FromClassHierarchyFind(type.BaseType, from);
                    }
                }
                else if (type.BaseType != null)
                {
                    return FromClassHierarchyFind(type.BaseType, from);
                }
            }

            return null;
        }
    }
}