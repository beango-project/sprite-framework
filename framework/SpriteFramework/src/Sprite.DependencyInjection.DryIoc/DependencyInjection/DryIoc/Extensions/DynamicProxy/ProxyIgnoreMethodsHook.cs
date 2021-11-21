using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace Sprite.DependencyInjection.DryIoc.Extensions.DynamicProxy
{
    public class ProxyIgnoreMethodsHook : AllMethodsHook
    {
        private readonly Predicate<MethodInfo> filter;

        public ProxyIgnoreMethodsHook(Predicate<MethodInfo> filter)
        {
            this.filter = filter;
        }


        public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            // Console.WriteLine("是否需要拦截______" +type.FullName+" : "+ methodInfo.Name);
            // if (filter(methodInfo))
            // {   Console.WriteLine("不需要拦截______" +type.FullName+" : "+ methodInfo.Name);
            //     return false;
            // }
            // Console.WriteLine("需要拦截______" +type.FullName+" : "+ methodInfo.Name);
            // // var asd= !filter(methodInfo);
            // if (filter(methodInfo))
            // {
            //     return false;
            // }
            // if (methodInfo.Name=="OnActionExecuted")
            // {
            //     return true;
            // }
                
            return SkippedTypes.Contains(methodInfo.DeclaringType);
        }
    }
}