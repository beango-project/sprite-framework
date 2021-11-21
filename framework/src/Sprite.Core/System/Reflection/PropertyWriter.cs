using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastExpressionCompiler.LightExpression;
using ImTools;

// System.Linq.Expressions;


namespace System.Reflection
{
    /// <summary>
    /// 属性设置器，针对反射进行性能优化以提供高性能的属性设置,尤其是第一次访问
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    public class PropertySetter<T>
    {
        public PropertySetter(object target, T tValue, PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
            var action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), target, methodInfo);
            action.Invoke(tValue);
        }
    }

    // public class PropertySetter
    // {
    //     private MethodInfo _methodInfo;
    //     private object _target;
    //
    //     public PropertySetter(object target, PropertyInfo propertyInfo)
    //     {
    //         _target = target;
    //         _methodInfo = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
    //     }
    //
    //     public void TrySet<T>(T tValue)
    //     {
    //         var action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), _target, _methodInfo);
    //         action.Invoke(tValue);
    //     }
    //     
    // }

    public class PropertyWriter
    {
        private static Dictionary<string, Action<object, object>> _imHashMap = new Dictionary<string, Action<object, object>>();


        public PropertyWriter(object target, object value, PropertyInfo propertyInfo)
        {
            var cacheName = target.GetType().FullName + value.GetType().FullName + propertyInfo.Name;
            if (!_imHashMap.TryGetValue(cacheName, out var callFunc))
            {
                var setAction = callFunc = CreateSetAction(propertyInfo);
                _imHashMap.TryAdd(cacheName, setAction);
            }

            callFunc.Invoke(target, value);
        }

        private Action<object, object> CreateSetAction(PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
            var target = Expression.Parameter(typeof(object), "target");
            var propValue = Expression.Parameter(typeof(object), "value");
            var setCall = Expression.Call(target, methodInfo, propValue);
            return Expression.Lambda<Action<object, object>>(setCall, target, propValue).CompileFast();
        }
        // public void Set(TValue tValue)
        // {
        //     _action.Invoke(_target, tValue);
        // }

        // private Action<T, TValue> CreateSetAction(PropertyInfo propertyInfo)
        // {
        //     var methodInfo = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
        //     var target = Expression.Parameter(typeof(T), "target");
        //     var propValue = Expression.Parameter(typeof(TValue), "value");
        //     var setCall = Expression.Call(target, methodInfo, propValue);
        //     return Expression.Lambda<Action<T, TValue>>(setCall, target, propValue).CompileFast();
        // }
    }


    /// <summary>
    /// Helper class to generate a action to set value to a object property
    /// </summary>
    public static class ValueSetter
    {
        /// <summary>
        /// Create a setter action to set value to a object property
        /// <example>
        /// var uriSetter = (Action&lt;HttpRequestMessage,Uri&gt;) ValueSetter.Create(typeof(HttpRequestMessage), typeof(Uri), typeof(HttpRequestMessage).GetProperty("RequestUri"))
        /// </example>
        /// </summary>
        /// <param name="objType">Type of target object</param>
        /// <param name="valueType">Type of target property</param>
        /// <param name="info">PropertyInfo of target property</param>
        /// <returns>A Action as Action&lt;objType,valueType&gt;. </returns>
        public static object Create(Type objType, Type valueType, PropertyInfo info)
        {
            var bodyExp = Expression.Call(
                typeof(ValueSetter<,,>).MakeGenericType(objType, info.PropertyType, valueType),
                nameof(ValueSetter<object, object, object>.GetSetter),
                Array.Empty<Type>(),
                Expression.Constant(info));
            var finalExp = Expression.Lambda<Func<object>>(bodyExp);
            var func = finalExp.CompileFast();
            var re = func.Invoke();
            return re;
        }

        public static void Change<TValue>(Type objType, TValue value, PropertyInfo info)
        {
            var bodyExp = Expression.Call(
                typeof(ValueSetter<,,>).MakeGenericType(objType, info.PropertyType, typeof(TValue)),
                nameof(ValueSetter<object, object, object>.GetSetter),
                Array.Empty<Type>(),
                Expression.Constant(info));
            var finalExp = Expression.Lambda<Func<object>>(bodyExp);
            var func = finalExp.CompileFast();
            var re = func.Invoke();
            re = value;
        }

        /// <summary>
        /// Create a setter action to set value to a object property
        /// <example>
        /// var uriSetter = (Action&lt;HttpRequestMessage,object&gt;) ValueSetter.Create(typeof(HttpRequestMessage), typeof(HttpRequestMessage).GetProperty("RequestUri"))
        /// </example>
        /// </summary>
        /// <param name="objType">Type of target object</param>
        /// <param name="info">PropertyInfo of target property</param>
        /// <returns>A Action as Action&lt;objType,object&gt;. </returns>
        public static object Create(Type objType, PropertyInfo info)
        {
            var bodyExp = Expression.Call(typeof(ValueSetter<>).MakeGenericType(objType),
                nameof(ValueSetter<object, object, object>.GetSetter),
                Array.Empty<Type>(),
                Expression.Constant(info));
            var finalExp = Expression.Lambda<Func<object>>(bodyExp);
            var func = finalExp.CompileFast();
            var re = func.Invoke();
            return re;
        }

        internal static SwitchCase CreateSetterCase<TTargetObject, TTargetValue>(PropertyInfo propertyInfo)
        {
            var sourceObjExp = Expression.Parameter(typeof(TTargetObject), "sourceObj");
            var valueExp = Expression.Parameter(typeof(TTargetValue), "value");
            var newValueExp = Expression.Convert(valueExp, propertyInfo.PropertyType);
            var bodyExp = Expression.Assign(Expression.Property(sourceObjExp, propertyInfo), newValueExp);
            var finalExp =
                Expression.Lambda<Action<TTargetObject, TTargetValue>>(bodyExp, sourceObjExp, valueExp);
            var getter = finalExp.CompileFast();
            var caseExp = Expression.Constant(propertyInfo);
            return Expression.SwitchCase(Expression.Constant(getter), caseExp);
        }

        internal static Func<PropertyInfo, Action<TTargetObject, TTargetValue>> CreateSetter<TTargetObject,
            TTargetValue>(
            IEnumerable<PropertyInfo> propertyInfos,
            Func<PropertyInfo, SwitchCase> caseFactory)
        {
            var pExp = Expression.Parameter(typeof(PropertyInfo), "info");
            var cases = propertyInfos.Select(caseFactory);

            var constantExpression = Expression.Constant(null, typeof(Action<TTargetObject, TTargetValue>));
            var switchExp = Expression.Switch(pExp, constantExpression, null, cases.ToArray());
            var funcExp = Expression.Lambda<Func<PropertyInfo, Action<TTargetObject, TTargetValue>>>(switchExp, pExp);
            var re = funcExp.CompileFast();
            return re;
        }
    }

    /// <summary>
    /// Value setter in generic format.
    /// </summary>
    /// <typeparam name="TTargetObject">Type of target object</typeparam>
    /// <typeparam name="TPropertyValue">Type of property</typeparam>
    /// <typeparam name="TTargetValue">Type of target value. This is used as type of action input value, it can be different from <typeparamref name="TPropertyValue"/>. You must confirm that <typeparamref name="TTargetValue"/> can be directly cast to <typeparamref name="TPropertyValue"/>, It will throw a exception otherwise.</typeparam>
    public static class ValueSetter<TTargetObject, TPropertyValue, TTargetValue>
    {
        private static readonly Func<PropertyInfo, Action<TTargetObject, TTargetValue>> Finder;

        static ValueSetter()
        {
            var propertyInfos = typeof(TTargetObject).GetRuntimeProperties()
                .Where(x => x.CanWrite)
                .Where(x => x.PropertyType == typeof(TPropertyValue));

            Finder = ValueSetter.CreateSetter<TTargetObject, TTargetValue>(propertyInfos,
                ValueSetter.CreateSetterCase<TTargetObject, TTargetValue>);
        }

        /// <summary>
        /// Create a setter action to set property value to a object property.
        /// <example>
        /// Action&lt;HttpRequestMessage,Uri&gt; uriSetter = ValueSetter&lt;HttpRequestMessage, Uri, Uri&gt;.GetSetter(typeof(HttpRequestMessage).GetProperty("RequestUri"))
        /// </example>
        /// </summary>
        /// <param name="info">PropertyInfo of target property</param>
        /// <returns>Func as a value setter</returns>
        public static Action<TTargetObject, TTargetValue> GetSetter(PropertyInfo info)
        {
            return Finder.Invoke(info);
        }
    }

    /// <summary>
    /// Value setter in no-generic format.
    /// </summary>
    /// <typeparam name="TTargetObject">Type of target object</typeparam>
    public static class ValueSetter<TTargetObject>
    {
        private static readonly Func<PropertyInfo, Action<TTargetObject, object>> Finder;

        static ValueSetter()
        {
            var propertyInfos = typeof(TTargetObject).GetRuntimeProperties()
                .Where(x => x.CanWrite);

            Finder = ValueSetter.CreateSetter<TTargetObject, object>(propertyInfos,
                ValueSetter.CreateSetterCase<TTargetObject, object>);
        }

        /// <summary>
        /// Create a setter action to set property value to a object property.
        /// <example>
        /// Action&lt;HttpRequestMessage,object&gt; uriSetter = ValueSetter&lt;HttpRequestMessage&gt;.GetSetter(typeof(HttpRequestMessage).GetProperty("RequestUri"))
        /// </example>
        /// </summary>
        /// <param name="info">PropertyInfo of target property</param>
        /// <returns>Func as a value setter</returns>
        public static Action<TTargetObject, object> GetSetter(PropertyInfo info)
        {
            return Finder.Invoke(info);
        }
    }
}