﻿/*
The MIT License (MIT)
Copyright (c) 2016-2021 Maksim Volkau
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection.DryIoc.Extensions.DynamicProxy;
using Sprite.DynamicProxy;

namespace DryIoc.Microsoft.DependencyInjection
{
    /// <summary>
    /// This DryIoc is supposed to be used with generic `IHostBuilder` like this:
    /// <code><![CDATA[
    /// public class Program
    /// {
    ///     public static async Task Main(string[] args) => 
    ///         await CreateHostBuilder(args).Build().RunAsync();
    /// 
    ///     Rules WithMyRules(Rules currentRules) => currentRules;
    /// 
    ///     public static IHostBuilder CreateHostBuilder(string[] args) =>
    ///         Host.CreateDefaultBuilder(args)
    ///             .UseServiceProviderFactory(new DryIocServiceProviderFactory(new Container(rules => WithMyRules(rules))))
    ///             .ConfigureWebHostDefaults(webBuilder =>
    ///             {
    ///                 webBuilder.UseStartup<Startup>();
    ///             });
    /// }
    /// ]]></code>
    /// Then register your services in `Startup.ConfigureContainer`.
    /// DON'T try to change the container rules there - they will be lost,
    /// instead pass the pre-configured container to `DryIocServiceProviderFactory` as in example above.
    /// By default container will use <see href="DryIoc.Rules.MicrosoftDependencyInjectionRules" />
    /// DON'T forget to add `services.AddControllers().AddControllersAsServices()` in `Startup.ConfigureServices`
    /// in order to access DryIoc diagnostics for controllers, property-injection, etc.
    /// </summary>
    public class DryIocServiceProviderFactory : IServiceProviderFactory<IContainer>
    {
        private readonly IContainer _container;
        private readonly Func<IRegistrator, ServiceDescriptor, bool> _registerDescriptor;
        private readonly RegistrySharing _registrySharing;

        /// <summary>
        /// We won't initialize the container here because it is logically expected to be done in `CreateBuilder`,
        /// so the factory constructor is just saving some options to use later.
        /// </summary>
        public DryIocServiceProviderFactory(
            IContainer container = null,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null) :
            this(container, RegistrySharing.CloneAndDropCache, registerDescriptor)
        {
        }

        /// <summary>
        /// `container` is the existing container which will be cloned with the MS.DI rules and its cache will be dropped,
        /// unless the `registrySharing` is set to the `RegistrySharing.Share` or to `RegistrySharing.CloneButKeepCache`.
        /// `registerDescriptor` is the custom service descriptor handler.
        /// </summary>
        public DryIocServiceProviderFactory(IContainer container, RegistrySharing registrySharing,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            _container = container;
            _registrySharing = registrySharing;
            _registerDescriptor = registerDescriptor;
        }

        /// <inheritdoc />
        public IContainer CreateBuilder(IServiceCollection services)
        {
            var container = _container;
            if (container == null)
            {
                container = new Container(Rules.MicrosoftDependencyInjectionRules);
            }
            else if (container.Rules != Rules.MicrosoftDependencyInjectionRules)
            {
                container = container.With(container.Rules.WithMicrosoftDependencyInjectionRules(),
                    container.ScopeContext, _registrySharing, container.SingletonScope);
            }

            container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));

            if (services != null)
            {
                container.Populate(services, _registerDescriptor);
            }

            return container;
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(IContainer container)
        {
            return container.BuildServiceProvider();
        }
    }

    /// <summary>
    /// Adapts DryIoc container to be used as MS.DI service provider, plus provides the helpers
    /// to simplify work with adapted container.
    /// </summary>
    public static class DryIocAdapter
    {
        /// <summary>
        /// Creates the container and the `IServiceProvider` because its implemented by `IContainer` -
        /// you get simply the best of both worlds.
        /// </summary>
        public static IContainer Create(
            IEnumerable<ServiceDescriptor> services,
            Func<IRegistrator, ServiceDescriptor, bool> registerService = null)
        {
            var container = new Container(Rules.MicrosoftDependencyInjectionRules);

            container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));
            container.Populate(services, registerService);

            return container;
        }

        /// <summary>
        /// Adapts passed <paramref name="container" /> to Microsoft.DependencyInjection conventions,
        /// registers DryIoc implementations of <see cref="IServiceProvider" /> and <see cref="IServiceScopeFactory" />,
        /// and returns NEW container.
        /// </summary>
        /// <param name="container">Source container to adapt.</param>
        /// <param name="descriptors">(optional) Specify service descriptors or use <see cref="Populate" /> later.</param>
        /// <param name="registerDescriptor">(optional) Custom registration action, should return true to skip normal registration.</param>
        /// <example>
        ///     <code><![CDATA[
        ///  
        ///      var container = new Container();
        /// 
        ///      // you may register the services here:
        ///      container.Register<IMyService, MyService>(Reuse.Scoped)
        ///  
        ///      var adaptedContainer = container.WithDependencyInjectionAdapter(services);
        ///      IServiceProvider serviceProvider = adaptedContainer; // the container implements IServiceProvider
        /// 
        /// ]]></code>
        /// </example>
        /// <remarks>You still need to Dispose adapted container at the end / application shutdown.</remarks>
        public static IContainer WithDependencyInjectionAdapter(this IContainer container,
            IEnumerable<ServiceDescriptor> descriptors = null,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            if (container.Rules != Rules.MicrosoftDependencyInjectionRules)
            {
                container = container.With(rules => rules.WithMicrosoftDependencyInjectionRules());
            }

            container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));

            // Registers service collection
            if (descriptors != null)
            {
                container.Populate(descriptors, registerDescriptor);
            }

            return container;
        }

        /// <summary>Adds services registered in <paramref name="compositionRootType" /> to container</summary>
        public static IContainer WithCompositionRoot(this IContainer container, Type compositionRootType)
        {
            container.Register(compositionRootType);
            container.Resolve(compositionRootType);
            return container;
        }

        /// <summary>Adds services registered in <typeparamref name="TCompositionRoot" /> to container</summary>
        public static IContainer WithCompositionRoot<TCompositionRoot>(this IContainer container)
        {
            return container.WithCompositionRoot(typeof(TCompositionRoot));
        }

        /// <summary>It does not really build anything, it just gets the `IServiceProvider` from the container.</summary>
        public static IServiceProvider BuildServiceProvider(this IContainer container)
        {
            return container.GetServiceProvider();
        }

        /// <summary>Just gets the `IServiceProvider` from the container.</summary>
        public static IServiceProvider GetServiceProvider(this IResolver container)
        {
            return container;
        }

        /// <summary>Facade to consolidate DryIoc registrations in <typeparamref name="TCompositionRoot" /></summary>
        /// <typeparam name="TCompositionRoot">
        /// The class will be created by container on Startup
        /// to enable registrations with injected <see cref="IRegistrator" /> or full <see cref="IContainer" />.
        /// </typeparam>
        /// <param name="container">Adapted container</param>
        /// <returns>Service provider</returns>
        /// <example>
        ///     <code><![CDATA[
        /// public class ExampleCompositionRoot
        /// {
        ///    // if you need the whole container then change parameter type from IRegistrator to IContainer
        ///    public ExampleCompositionRoot(IRegistrator r)
        ///    {
        ///        r.Register<ISingletonService, SingletonService>(Reuse.Singleton);
        ///        r.Register<ITransientService, TransientService>(Reuse.Transient);
        ///        r.Register<IScopedService, ScopedService>(Reuse.InCurrentScope);
        ///    }
        /// }
        /// ]]></code>
        /// </example>
        public static IServiceProvider ConfigureServiceProvider<TCompositionRoot>(this IContainer container)
        {
            return container.WithCompositionRoot<TCompositionRoot>().GetServiceProvider();
        }

        /// <summary>Registers service descriptors into container. May be called multiple times with different service collections.</summary>
        /// <param name="container">The container.</param>
        /// <param name="descriptors">The service descriptors.</param>
        /// <param name="registerDescriptor">(optional) Custom registration action, should return true to skip normal registration.</param>
        /// <example>
        ///     <code><![CDATA[
        ///     // example of normal descriptor registration together with factory method registration for SomeService.
        ///     container.Populate(services, (r, service) => {
        ///         if (service.ServiceType == typeof(SomeService)) {
        ///             r.Register<SomeService>(Made.Of(() => CreateCustomService()), Reuse.Singleton);
        ///             return true;
        ///         };
        ///         return false; // fallback to normal registrations for the rest of the descriptors.
        ///     });
        /// ]]></code>
        /// </example>
        public static void Populate(this IContainer container, IEnumerable<ServiceDescriptor> descriptors,
            Func<IRegistrator, ServiceDescriptor, bool> registerDescriptor = null)
        {
            if (registerDescriptor == null)
            {
                foreach (var descriptor in descriptors)
                {
                    container.RegisterDescriptor(descriptor);
                }
            }
            else
            {
                foreach (var descriptor in descriptors)
                {
                    if (!registerDescriptor(container, descriptor))
                    {
                        container.RegisterDescriptor(descriptor);
                    }
                }
            }

            RegisterIntercepts(container, descriptors); //注册动态代理的拦截器
        }

        /// <summary>
        /// Register dynamicProxy intercepts
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="descriptors">The service descriptors.</param>
        private static void RegisterIntercepts(IContainer container, IEnumerable<ServiceDescriptor> descriptors)
        {
            var interceptorOptions = container.Resolve<IOptions<InterceptorOptions>>()?.Value;
            foreach (var bindingContext in interceptorOptions.InterceptorBindings)
            {
                if (bindingContext.Selector != null)
                {
                    // var serviceDescriptors = descriptors.Where(x =>
                    //     x.ServiceType.FullName == "WebApplicationTest.Services.V1.IIdentityAppService" ||
                    //     x.ServiceType.FullName == "WebApplicationTest.Services.V1.IdentityAppService");

                    var serviceDescriptors = descriptors.Where(s => bindingContext.Selector(s.ServiceType));

                    if (serviceDescriptors.Any())
                    {
                        foreach (var serviceDescriptor in serviceDescriptors)
                        {
                            if (bindingContext.IgnoreMethods != null)
                            {
                                var proxyGenerationOptions = new ProxyGenerationOptions(new ProxyIgnoreMethodsHook(bindingContext.IgnoreMethods));
                                container.InterceptAsync(serviceDescriptor.ServiceType, bindingContext.Interceptor);
                            }
                            else
                            {
                                container.InterceptAsync(serviceDescriptor.ServiceType, bindingContext.Interceptor);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Uses passed descriptor to register service in container:
        /// maps DI Lifetime to DryIoc Reuse,
        /// and DI registration type to corresponding DryIoc Register, RegisterDelegate or RegisterInstance.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="descriptor">Service descriptor.</param>
        public static void RegisterDescriptor(this IContainer container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
            {
                var reuse = descriptor.Lifetime == ServiceLifetime.Singleton ? Reuse.Singleton
                    : descriptor.Lifetime == ServiceLifetime.Scoped ? Reuse.ScopedOrSingleton
                    : Reuse.Transient;

                container.Register(descriptor.ServiceType, descriptor.ImplementationType, reuse);
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var reuse = descriptor.Lifetime == ServiceLifetime.Singleton ? Reuse.Singleton
                    : descriptor.Lifetime == ServiceLifetime.Scoped ? Reuse.ScopedOrSingleton
                    : Reuse.Transient;

                container.RegisterDelegate(true, descriptor.ServiceType,
                    descriptor.ImplementationFactory,
                    reuse);
            }
            else
            {
                container.RegisterInstance(true, descriptor.ServiceType, descriptor.ImplementationInstance);
            }
        }
    }

    /// <summary>Creates/opens new scope in passed scoped container.</summary>
    public sealed class DryIocServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IResolverContext _scopedResolver;

        /// <summary>Stores passed scoped container to open nested scope.</summary>
        /// <param name="scopedResolver">Scoped container to be used to create nested scope.</param>
        public DryIocServiceScopeFactory(IResolverContext scopedResolver)
        {
            _scopedResolver = scopedResolver;
        }

        /// <summary>Opens scope and wraps it into DI <see cref="IServiceScope" /> interface.</summary>
        /// <returns>DI wrapper of opened scope.</returns>
        public IServiceScope CreateScope()
        {
            var r = _scopedResolver;
            var scope = r.ScopeContext == null ? new Scope(r.CurrentScope) : r.ScopeContext.SetCurrent(p => new Scope(p));
            return new DryIocServiceScope(r.WithCurrentScope(scope));
        }
    }

    /// <summary>Bare-bones IServiceScope implementations</summary>
    public sealed class DryIocServiceScope : IServiceScope
    {
        private readonly IResolverContext _resolverContext;

        /// <summary>Creating from resolver context</summary>
        public DryIocServiceScope(IResolverContext resolverContext)
        {
            _resolverContext = resolverContext;
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider => _resolverContext;

        /// <summary>Disposes the underlying resolver context</summary>
        public void Dispose()
        {
            _resolverContext.Dispose();
        }
    }
}