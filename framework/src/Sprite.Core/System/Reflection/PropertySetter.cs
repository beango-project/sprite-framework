using System.Collections.Generic;
using FastExpressionCompiler.LightExpression;

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


    public class PropertySetterWithExpression
    {
        private static Dictionary<string, Action<object, object>> _imHashMap = new Dictionary<string, Action<object, object>>();


        public PropertySetterWithExpression(object target, object value, PropertyInfo propertyInfo)
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
    }
}