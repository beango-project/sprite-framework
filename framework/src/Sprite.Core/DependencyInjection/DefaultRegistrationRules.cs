using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.DependencyInjection.Attributes;
using Sprite.ObjectMapping;

namespace Sprite.DependencyInjection
{
    public class DefaultRegistrationRules : RegistrationRulesBase, IRegistrationRules
    {
        public override void AddFromTypeOf(IServiceCollection services, Type type)
        {
            var registerAttribute = type.GetCustomAttribute<ComponentAttribute>(true);
            var serviceLifetime = GetLifeTime(type, registerAttribute);
            if (serviceLifetime == null)
            {
                return;
            }


            var exportServiceTypes = GetExportServiceTypes(type);

            // TriggerServiceExposing(services, type, exportServiceTypes);

            foreach (var exportServiceType in exportServiceTypes)
            {
                var serviceDescriptor = CreateServiceDescriptor(
                    type,
                    exportServiceType,
                    exportServiceTypes,
                    serviceLifetime.Value
                );

                if (registerAttribute?.Replace == true)
                {
                    services.Replace(serviceDescriptor);
                }
                else if (registerAttribute?.TryRegister == true)
                {
                    services.TryAdd(serviceDescriptor);
                }
                else
                {
                    services.Add(serviceDescriptor);
                }
            }
        }

        protected virtual List<Type> GetExportServiceTypes(Type type)
        {
            var serviceTypes = ExportServiceScanner.GetServices(type);
            return serviceTypes;
        }

        protected virtual ServiceDescriptor CreateServiceDescriptor(Type implementationType, Type exposingServiceType, List<Type> allExposingServiceTypes, ServiceLifetime lifeTime)
        {
            if (lifeTime.IsIn(ServiceLifetime.Singleton, ServiceLifetime.Scoped))
            {
                var exportedServiceType = DetermineExportedServiceType(
                    implementationType,
                    exposingServiceType,
                    allExposingServiceTypes
                );

                if (exportedServiceType != null)
                {
                    return ServiceDescriptor.Describe(
                        exposingServiceType,
                        provider => provider.GetService(exportedServiceType),
                        lifeTime
                    );
                }
                // if (exportedServiceType != null)
                // {
                //     return ServiceDescriptor.Describe(exposingServiceType, exportedServiceType, lifeTime);
                // }
            }

            return ServiceDescriptor.Describe(
                exposingServiceType,
                implementationType,
                lifeTime
            );
        }

        /// <summary>
        /// 确定导出的服务类型
        /// </summary>
        /// <param name="implementationType">实现类型</param>
        /// <param name="exportedType">导出类型</param>
        /// <param name="exportedTypes">所有导出的服务类型</param>
        /// <returns></returns>
        protected virtual Type DetermineExportedServiceType(Type implementationType, Type exportedType, List<Type> exportedTypes)
        {
            //导出的类型小于2个
            if (exportedTypes.Count < 2)
            {
                return null;
            }

            //导出类型等于实现类型
            if (exportedType == implementationType)
            {
                return null;
            }

            if (exportedTypes.Contains(implementationType))
            {
                return implementationType;
            }

            return exportedTypes.FirstOrDefault(t => t != exportedType && exportedType.IsAssignableFrom(t));
        }

        protected virtual ServiceLifetime? GetLifeTime(Type type, ComponentAttribute attribute)
        {
            return attribute?.Scope ?? GetLifetimeFromClassHierarchy(type);
        }

        private ServiceLifetime? GetLifetimeFromClassHierarchy(Type type)
        {
            if (typeof(ITransientInjection).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Transient;
            }

            if (typeof(ISingletonInjection).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Singleton;
            }

            if (typeof(IScopeInjection).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Scoped;
            }

            return null;
        }

        protected virtual void TriggerServiceExposing(IServiceCollection services, Type implementationType, List<Type> serviceTypes)
        {
            var exposeActions = services.GetExportServiceActivator();
            if (exposeActions.Any())
            {
                var args = new ExportServiceArgs(implementationType, serviceTypes);
                foreach (var action in exposeActions)
                {
                    action(args);
                }
            }
        }
    }
}