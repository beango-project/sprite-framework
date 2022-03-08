using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using ImmediateReflection;

namespace System.Reflection
{
    public static class ReflectionHelper
    {
        private static ConcurrentDictionary<string, PropertyInfo> ObjectPropertyCache = new();

        public static void TrySetProperty<TObject, TValue>(TObject obj, TValue value, Expression<Func<TObject, TValue>> propertySelector)
        {
            if (propertySelector.Body.NodeType != ExpressionType.MemberAccess)
            {
                return;
            }

            if (propertySelector.Body is MemberExpression memberExpression)
            {
                // Ex x => x.Name == memberExpression.Member.Name && x.GetSetMethod(true) != null.v

                Expression<Func<PropertyInfo, bool>> expTemp = x => x.Name == memberExpression.Member.Name && x.GetSetMethod(true) != null;
                var compileFast = expTemp.CompileFast();
                var prop = obj.GetType().GetProperties().FirstOrDefault(compileFast);

                if (prop != null)
                {
                    prop.SetValue(obj, value);
                }
            }
        }


        public static IEnumerable<TAttribute> GetAttributesOfMemberInfo<TAttribute>(MemberInfo memberInfo, bool includeDeclaringType = true, bool inherit = true)
            where TAttribute : Attribute
        {
            var customAttributes = memberInfo.GetAllImmediateAttributes(inherit).OfType<TAttribute>();
            IEnumerable<TAttribute> declaringTypeCustomAttributes = null;
            if (includeDeclaringType)
            {
                return declaringTypeCustomAttributes != null
                    ? customAttributes.Concat(declaringTypeCustomAttributes).Distinct()
                    : customAttributes;
            }

            return customAttributes;
        }

        public static TAttribute GetSingleAttributeOrDefaultByFullSearch<TAttribute>(TypeInfo info)
            where TAttribute : Attribute
        {
            var attributeType = typeof(TAttribute);
            if (info.IsDefinedAttribute(attributeType, true))
            {
                return info.GetImmediateAttributes(attributeType, true).Cast<TAttribute>().First();
            }

            foreach (var implInter in info.ImplementedInterfaces)
            {
                var res = GetSingleAttributeOrDefaultByFullSearch<TAttribute>(implInter.GetTypeInfo());
                if (res != null)
                {
                    return res;
                }
            }

            return null;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }
    }
}