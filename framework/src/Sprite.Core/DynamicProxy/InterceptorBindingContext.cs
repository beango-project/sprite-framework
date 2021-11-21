using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sprite.DynamicProxy
{
    /// <summary>
    /// 拦截器绑定上下文
    /// </summary>
    public class InterceptorBindingContext
    {
        public InterceptorBindingContext()
        {
            Types = new HashSet<Type>();
        }

        public Type Interceptor { get; set; }

        public Func<Type, bool> Selector { get; set; }

        public HashSet<Type> Types { get; set; }

        public Predicate<MethodInfo> IgnoreMethods { get; set; }

        public void Set(Type type)
        {
            if (Selector(type))
            {
                Types.AddIfNotContains(type);
            }
        }
    }
}