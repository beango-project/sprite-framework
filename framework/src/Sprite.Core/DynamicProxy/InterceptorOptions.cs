using System;
using System.Collections.Generic;

namespace Sprite.DynamicProxy
{
    public class InterceptorOptions
    {
        public InterceptorOptions()
        {
            InterceptorBindings = new List<InterceptorBindingContext>();
            IgnoreType = new List<Type>();
        }

        // private readonly List<InterceptorBindingContext> _interceptorBindings;
        public List<InterceptorBindingContext> InterceptorBindings { get; }


        public List<Type> IgnoreType { get; set; }

        public InterceptorOptions BindingIntercept(Action<InterceptorBindingContext> action)
        {
            var context = new InterceptorBindingContext();
            action?.Invoke(context);
            InterceptorBindings.Add(context);
            return this;
        }
    }

    public class AspectAttribute : Attribute
    {
    }
}