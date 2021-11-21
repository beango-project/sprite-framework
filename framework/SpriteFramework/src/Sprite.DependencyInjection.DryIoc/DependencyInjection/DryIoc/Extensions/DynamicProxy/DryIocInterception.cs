using System;
using System.Linq;
using Castle.DynamicProxy;
using DryIoc;
using Sprite.Castle.DynamicProxy;
using Sprite.DynamicProxy;

namespace Sprite.DependencyInjection.DryIoc.Extensions.DynamicProxy
{
    public static class DryIocInterception
    {
        private static readonly DefaultProxyBuilder _proxyBuilder = new();


        public static void Intercept(this IContainer container, Type serviceType, Type interceptorType, ProxyGenerationOptions options = null, object serviceKey = null)
        {
            Type proxyType;
            if (serviceType.IsInterface())
            {
                var instance = container.Resolve(serviceType);
                var proxiedInterfaces = instance
                    .GetType()
                    .GetInterfaces()
                    .Where(ProxyUtil.IsAccessible)
                    .ToArray();
                var theInterface = proxiedInterfaces.First();
                var interfaces = proxiedInterfaces.Skip(1).ToArray();


                proxyType = options == null
                    ? _proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(
                        theInterface, interfaces, ProxyGenerationOptions.Default)
                    : _proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(
                        theInterface, interfaces, options);
            }
            else if (serviceType.IsClass())
            {
                if (options == null)
                {
                    proxyType = _proxyBuilder.CreateClassProxyType(serviceType, Array.Empty<Type>(), ProxyGenerationOptions.Default);
                }
                else
                {
                    proxyType = _proxyBuilder.CreateClassProxyType(serviceType, Array.Empty<Type>(), options);
                }
                // proxyType = options == null
                //     ? _proxyBuilder.CreateClassProxyType(
                //         serviceType, ArrayTools.Empty<Type>(), ProxyGenerationOptions.Default)
                //     : _proxyBuilder.CreateClassProxyType(
                //         serviceType, ArrayTools.Empty<Type>(), options);
            }
            else
            {
                throw new ArgumentException(
                    $"Intercepted service type {serviceType} is not a supported, cause it is nor a class nor an interface");
            }

            // var decoratorSetup = serviceKey == null
            //     ? Setup.DecoratorWith(useDecorateeReuse: true)
            //     : Setup.DecoratorWith(r => serviceKey.Equals(r.ServiceKey), useDecorateeReuse: true);
            var decoratorSetup = serviceKey == null
                ? Setup.DecoratorOf(serviceType, useDecorateeReuse: true)
                : Setup.DecoratorOf(serviceType, decorateeServiceKey: serviceKey, useDecorateeReuse: true);


            container.Register(serviceType, proxyType,
                made: Made.Of(type => type.GetConstructors().SingleOrDefault(c => c.GetParameters().Length != 0),
                    Parameters.Of.Type<IInterceptor[]>(interceptorType.MakeArrayType())),
                setup: decoratorSetup);
        }

        public static void Intercept<TService, TInterceptor>(this IContainer container, ProxyGenerationOptions options = null, object serviceKey = null)
            where TInterceptor : class, IInterceptor
        {
            Intercept(container, typeof(TService), typeof(TInterceptor), options, serviceKey);
        }

        public static void InterceptAsync(this IContainer container, Type serviceType, Type interceptorType, ProxyGenerationOptions options = null, object serviceKey = null)
        {
            var makeGenericType = typeof(CastleAsyncDeterminationInterceptor<>).MakeGenericType(interceptorType);
            if (!container.IsRegistered(makeGenericType))
            {
                container.Register(makeGenericType);
            }

            container.Intercept(serviceType, makeGenericType, options, serviceKey);
        }

        public static void InterceptAsync<TService, TInterceptor>(this IContainer container, ProxyGenerationOptions options = null, object serviceKey = null)
            where TInterceptor : class, IAspectInterceptor
        {
            var makeGenericType = typeof(CastleAsyncDeterminationInterceptor<>).MakeGenericType(typeof(TInterceptor));
            container.Register(makeGenericType);
            container.Intercept<TService, CastleAsyncDeterminationInterceptor<TInterceptor>>(options, serviceKey);
        }
    }
}