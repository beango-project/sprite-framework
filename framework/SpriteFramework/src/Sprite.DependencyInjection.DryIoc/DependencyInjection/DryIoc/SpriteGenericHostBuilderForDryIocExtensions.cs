using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.DependencyInjection.DryIoc
{
    public static class SpriteGenericHostBuilderForDryIocExtensions
    {
        private static readonly PropertiesAndFieldsSelector _getImportedPropertiesAndFields =
            PropertiesAndFields.All(serviceInfo: GetImportedPropertiesAndFieldsOnly);

        public static IHostBuilder UseSpriteServiceProviderFactory(this IHostBuilder hostBuilder)
        {
            var factory = CreateDryIocServiceProviderFactory();
            return hostBuilder.UseServiceProviderFactory(factory);
        }

        public static DryIocServiceProviderFactory CreateDryIocServiceProviderFactory()
        {
            IContainer container =
                new Container(Rules.MicrosoftDependencyInjectionRules.WithAutoConcreteTypeResolution().With(propertiesAndFields: _getImportedPropertiesAndFields));
            return new DryIocServiceProviderFactory(container);
        }

        private static PropertyOrFieldServiceInfo GetImportedPropertiesAndFieldsOnly(MemberInfo member, Request request)
        {
            var attribute = member.GetAttributeWithDefined<AutowiredAttribute>();

            if (attribute == null)
            {
                return null;
            }

            var importedPropertiesAndFieldsOnly = PropertyOrFieldServiceInfo.Of(member);

            if (attribute.Type != null)
            {
                return importedPropertiesAndFieldsOnly.WithDetails(ServiceDetails.Of(attribute.Type));
            }

            if (attribute.Key != null)
            {
                return importedPropertiesAndFieldsOnly.WithDetails(ServiceDetails.Of(serviceKey: attribute.Key));
            }
            
            return importedPropertiesAndFieldsOnly;
        }

        private static ServiceDetails GetImportDetails(Type type, Attribute[] attributes, Request request)
        {
            // object serviceKey;
            // Type requiredServiceType;
            // var ifUnresolved = IfUnresolved.Throw;

            var autowired = GetSingleAttributeOrDefault<AutowiredAttribute>(attributes);

            if (autowired != null)
            {
                if (autowired.Type != null)
                {
                    var resolve = request.Container.GetServiceRegistrations().FirstOrDefault(x => x.ImplementationType == autowired.Type);


                    return resolve.ToServiceInfo().Details;
                }

                return ServiceDetails.Of(request.RequiredServiceType);
            }

            return null;
        }

        private static TAttribute GetSingleAttributeOrDefault<TAttribute>(Attribute[] attributes) where TAttribute : Attribute
        {
            TAttribute attr = null;
            for (var i = 0; i < attributes.Length && attr == null; i++)
            {
                attr = attributes[i] as TAttribute;
            }

            return attr;
        }
    }
}