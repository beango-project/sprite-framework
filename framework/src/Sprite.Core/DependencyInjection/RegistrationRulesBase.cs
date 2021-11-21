using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.DependencyInjection
{
    public abstract class RegistrationRulesBase : IRegistrationRules
    {
        public virtual void AddFromAssemblyOf(IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes().AsParallel().Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType).ToArray();
            AddFromTypesOf(services, types);
        }

        public virtual void AddFromTypesOf(IServiceCollection services, Type[] types)
        {
            foreach (var type in types)
            {
                AddFromTypeOf(services, type);
            }
        }

        public abstract void AddFromTypeOf(IServiceCollection services, Type type);

        // protected virtual void AddInterceptTypes(IServiceCollection services, Type[]types)
        // {
        //     InterceptorOptions interceptorOptions = new InterceptorOptions();
        //     foreach (var type in types)
        //     {
        //        interceptorOptions.Interceptors.Add(new InterceptorBindingContext()
        //        {
        //            Interceptor = type
        //        });
        //     }
        // }
    }
}