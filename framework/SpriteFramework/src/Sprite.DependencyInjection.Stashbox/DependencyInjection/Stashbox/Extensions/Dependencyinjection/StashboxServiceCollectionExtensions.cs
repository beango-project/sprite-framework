using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sprite;
using Sprite.DynamicProxy;
using Stashbox;
using Stashbox.Configuration;
using Stashbox.Extensions.Dependencyinjection;
using Stashbox.Lifetime;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Stashbox related <see cref="IServiceCollection" /> extensions.
    /// </summary>
    public static class StashboxServiceCollectionExtensions
    {
        public static ProxyGenerator ProxyGenerator = new();

        /// <summary>
        /// Replaces the default <see cref="IServiceProviderFactory{TContainerBuilder}" /> with a factory which uses Stashbox as the default <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer" />.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddStashbox(this IServiceCollection services, Action<IStashboxContainer> configure = null)
        {
            return services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IStashboxContainer>>(new StashboxServiceProviderFactory(configure)));
        }

        /// <summary>
        /// Replaces the default <see cref="IServiceProviderFactory{TContainerBuilder}" /> with a factory which uses Stashbox as the default <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer" /> instance to use.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddStashbox(this IServiceCollection services, IStashboxContainer container)
        {
            return services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IStashboxContainer>>(new StashboxServiceProviderFactory(container)));
        }

        /// <summary>
        /// Registers the services from the <paramref name="serviceCollection" /> and creates a service provider which uses Stashbox.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer" />.</param>
        /// <returns>The configured <see cref="IServiceProvider" /> instance.</returns>
        public static IServiceProvider UseStashbox(this IServiceCollection serviceCollection, Action<IStashboxContainer> configure = null)
        {
            return serviceCollection.CreateBuilder(configure);
        }


        /// <summary>
        /// Registers the services from the <paramref name="serviceCollection" /> and creates a service provider which uses Stashbox.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer" /> instance to use.</param>
        /// <returns>The configured <see cref="IServiceProvider" /> instance.</returns>
        public static IServiceProvider UseStashbox(this IServiceCollection serviceCollection, IStashboxContainer container)
        {
            return serviceCollection.CreateBuilder(container);
        }

        /// <summary>
        /// Registers the services from the <paramref name="serviceCollection" /> and returns with the prepared <see cref="IStashboxContainer" />.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer" />.</param>
        /// <returns>The configured <see cref="IStashboxContainer" /> instance.</returns>
        public static IStashboxContainer CreateBuilder(this IServiceCollection serviceCollection, Action<IStashboxContainer> configure = null)
        {
            return PrepareContainer(serviceCollection, configure);
        }

        /// <summary>
        /// Registers the services from the <paramref name="serviceCollection" /> and returns with the prepared <see cref="IStashboxContainer" />.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer" /> instance to use.</param>
        /// <returns>The configured <see cref="IStashboxContainer" /> instance.</returns>
        public static IStashboxContainer CreateBuilder(this IServiceCollection serviceCollection, IStashboxContainer container)
        {
            return PrepareContainer(serviceCollection, null, container);
        }

        /// <summary>
        /// Registers the given services into the container.
        /// </summary>
        /// <param name="container">The <see cref="IStashboxContainer" />.</param>
        /// <param name="services">The service descriptors.</param>
        public static void RegisterServiceDescriptors(this IDependencyRegistrator container, IEnumerable<ServiceDescriptor> services)
        {
            foreach (var descriptor in services)
            {
                var lifetime = ChooseLifetime(descriptor.Lifetime);
                if (descriptor.ImplementationType != null)
                {
                    container.Register(descriptor.ServiceType,
                        descriptor.ImplementationType,
                        context => { context.WithLifetime(lifetime); });
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    container.Register(descriptor.ServiceType,
                        context => context
                            .WithFactory(descriptor.ImplementationFactory)
                            .WithLifetime(lifetime));
                }
                else
                {
                    container.RegisterInstance(descriptor.ImplementationInstance, descriptor.ServiceType);
                }
            }
        }

        private static LifetimeDescriptor ChooseLifetime(ServiceLifetime serviceLifetime)
        {
            return serviceLifetime switch
            {
                ServiceLifetime.Scoped => Lifetimes.Scoped,
                ServiceLifetime.Singleton => Lifetimes.Singleton,
                ServiceLifetime.Transient => Lifetimes.Transient,
                _ => throw new ArgumentOutOfRangeException(nameof(serviceLifetime))
            };
        }

        private static IStashboxContainer PrepareContainer(IServiceCollection services,
            Action<IStashboxContainer> configure = null, IStashboxContainer stashboxContainer = null)
        {
            var container = stashboxContainer ?? new StashboxContainer();

            container.Configure(config => config
                .WithDisposableTransientTracking()
                .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));

            configure?.Invoke(container);

            container.RegisterInstance<IServiceScopeFactory>(new StashboxServiceScopeFactory(container));
            container.Register<IServiceProvider>(c => c.WithFactory(r => new StashboxRequiredServiceProvider(r)));

            container.RegisterServiceDescriptors(services);

            return container;
        }

        private static void RegisterInterceptor(this IStashboxContainer container, IEnumerable<ServiceDescriptor> services)
        {
            var interceptorOptions = container.Resolve<IOptions<InterceptorOptions>>().Value;
            Check.NotNull(interceptorOptions, nameof(interceptorOptions));
            var interceptor = new List<object>();

            foreach (var bindingContext in interceptorOptions.InterceptorBindings)
            {
                var res = container.Resolve(bindingContext.Interceptor);
                if (res != null)
                {
                    interceptor.Add(res);
                }
            }

            foreach (var descriptor in services)
            {
                if (descriptor.ServiceType.IsInterface)
                {
                    // registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
                }
            }

            var filterResult = new List<ServiceDescriptor>();
            foreach (var bindingContext in interceptorOptions.InterceptorBindings)
            {
                // services.Select(x => x.ServiceType).Where(bindingContext.Selector);
                IEnumerable<ServiceDescriptor> res = null;
                if (bindingContext.Selector != null)
                {
                    res = services.Where(x => bindingContext.Selector(x.ServiceType) ||
                                              bindingContext.Selector(x.ImplementationType) ||
                                              bindingContext.Selector(x.ImplementationInstance.GetType()));
                }

                foreach (var service in res)
                {
                    if (service.ServiceType.IsInterface)
                    {
                        //TODO: 获取所有接口，开启接口类型代理
                        //是接口,开启接口代理
                        var proxiedInterfaces = service.ImplementationInstance.GetType().GetInterfaces().Where(ProxyUtil.IsAccessible);
                        // proxiedInterfaces
                        var theInterface = proxiedInterfaces.First();
                        var interfaces = proxiedInterfaces.Skip(1).ToArray();
                        // var interfaceProxyTypeWithTargetInterface =
                        //     ProxyGenerator.ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(typeof(IInterceptor), new Type[0], ProxyGenerationOptions.Default);
                        if (service.ImplementationType != null)
                        {
                            var proxy = ProxyGenerator.ProxyBuilder.CreateInterfaceProxyTypeWithTarget(service.ServiceType, new Type[0], service.ImplementationType,
                                ProxyGenerationOptions.Default);
                            container.RegisterDecorator(bindingContext.Interceptor, proxy);
                        }
                        else if (service.ImplementationInstance != null)
                        {
                            var proxy = ProxyGenerator.CreateInterfaceProxyWithTarget(service.ServiceType, new Type[0], service.ImplementationInstance,
                                ProxyGenerationOptions.Default);
                            container.RegisterDecorator(bindingContext.Interceptor, proxy.GetType());
                        }
                    }
                }
            }

            // foreach (var descriptor in services)
            // {
            //     /*TODO: 如果服务类型是接口，则启用接口代理，如果接口代理？是否需要获取实例？factory该如何处理？
            //      * 应该有一种方式能够获取到所有的拦截器，可以从ioc或是其他集合容器中获取？
            //     */
            //     if (descriptor.ServiceType.IsInterface) // descriptor.ImplementationInstance  should not null ?
            //     {
            //         // //TODO: 获取所有接口，开启接口类型代理
            //         var proxiedInterfaces = descriptor.ImplementationInstance.GetType().GetInterfaces().Where(ProxyUtil.IsAccessible);
            //         // proxiedInterfaces
            //         var theInterface = proxiedInterfaces.First();
            //         var interfaces = proxiedInterfaces.Skip(1).ToArray();
            //
            //         CastleProxyGenerator.Build().CreateInterfaceProxyWithTarget(theInterface, interfaces, descriptor.ImplementationInstance);
            //     }
            // }
        }
    }
}