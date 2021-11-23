using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CommonTests.DependencyInjection
{
    public static class ServiceCollectionWithTestExtensions
    {
        public static void ShouldBeTransient(this IServiceCollection services, Type serviceType, Type ImpType = null)
        {
            var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == serviceType);
            serviceDescriptor.ImplementationInstance.ShouldBeNull();
            if (ImpType == null)
            {
                serviceDescriptor.ImplementationType.ShouldBe(serviceType);
            }
            else
            {
                serviceDescriptor.ImplementationType.ShouldBe(ImpType);
            }

            serviceDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
        }
        
        public static void ShouldBeScope(this IServiceCollection services, Type serviceType, Type ImpType = null)
        {
            var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == serviceType);
            serviceDescriptor.ImplementationInstance.ShouldBeNull();
            if (ImpType == null)
            {
                serviceDescriptor.ImplementationType.ShouldBe(serviceType);
            }
            else
            {
                serviceDescriptor.ImplementationType.ShouldBe(ImpType);
            }

            serviceDescriptor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        }
        
        public static void ShouldBeSingleton(this IServiceCollection services, Type serviceType, Type ImpType = null)
        {
            var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == serviceType);
            serviceDescriptor.ImplementationInstance.ShouldBeNull();
            if (ImpType == null)
            {
                serviceDescriptor.ImplementationType.ShouldBe(serviceType);
            }
            else
            {
                serviceDescriptor.ImplementationType.ShouldBe(ImpType);
            }

            serviceDescriptor.Lifetime.ShouldBe(ServiceLifetime.Singleton);
        }
    }
}