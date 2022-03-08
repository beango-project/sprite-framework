// System.Linq.Expressions;


using AspectCore.Extensions.Reflection;

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
            propertyInfo.GetReflector().SetValue(target, tValue);
        }

        #region Outdated method

        // Replace with a better-performing method

        // public PropertySetter(object target, T tValue, PropertyInfo propertyInfo)
        // {
        //     var methodInfo = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
        //     var action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), target, methodInfo);
        //     action.Invoke(tValue);
        // }

        #endregion
    }


    // public class PropertySetterWithExpression
    // {
    //     private static ImHashMap<string, Action<object, object>> _imHashMap = ImHashMap<string, Action<object, object>>.Empty;
    //
    //
    //     public PropertySetterWithExpression(object target, object value, PropertyInfo propertyInfo)
    //     {
    //         var cacheName = target.GetType().FullName + value.GetType().FullName + propertyInfo.Name;
    //         if (!_imHashMap.TryFind(cacheName, out var callFunc))
    //         {
    //             var setAction = callFunc = CreateSetAction(propertyInfo);
    //             _imHashMap = _imHashMap.AddOrUpdate(cacheName, setAction);
    //         }
    //
    //         callFunc.Invoke(target, value);
    //     }
    //
    //     private Action<object, object> CreateSetAction(PropertyInfo propertyInfo)
    //     {
    //         var methodInfo = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
    //         var target = Expression.Parameter(typeof(object), "target");
    //         var propValue = Expression.Parameter(typeof(object), "value");
    //         var setCall = Expression.Call(target, methodInfo, propValue);
    //         return Expression.Lambda<Action<object, object>>(setCall, target, propValue).CompileFast();
    //     }
    // }
}