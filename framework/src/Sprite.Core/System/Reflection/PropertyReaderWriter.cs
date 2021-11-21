using System;
using System.Reflection;

namespace System.Reflection
{
    /// <summary>
    /// 属性读写器，高性能的读取和设置属性
    /// </summary>
    public class PropertyReaderWriter<T>
    {
        private Action<T> setter;
        private Func<T> getter;

        public T Value
        {
            get => getter();
            set => setter(value);
        }

        public PropertyReaderWriter(object target, PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
            var @delegate = Delegate.CreateDelegate(typeof(Action<T>), target, methodInfo);
            setter = (Action<T>) @delegate;

            methodInfo = propertyInfo.GetGetMethod() ?? propertyInfo.GetGetMethod(true);
            @delegate = Delegate.CreateDelegate(typeof(Func<T>), target, methodInfo);
            getter = (Func<T>) @delegate;
        }
    }
}