using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.DependencyInjection.DryIoc
{
    public class DryIocServiceProviderAdapter : ServiceProviderAdapter<IContainer>
    {
        private static readonly PropertiesAndFieldsSelector _getImportedPropertiesAndFields =
            PropertiesAndFields.All(serviceInfo: GetImportedPropertiesAndFieldsOnly);

        private IContainer _container;

        public override IContainer Container => _container;

        public override IContainer Initialization()
        {
            _container = new Container(Rules.MicrosoftDependencyInjectionRules.With(propertiesAndFields: _getImportedPropertiesAndFields)
                .WithDynamicRegistrationsAsFallback(Rules.ConcreteTypeDynamicRegistrations()));
            return _container;
        }

        public override IServiceProviderFactory<IContainer> CreateServiceProviderFactory()
        {
            return new DryIocServiceProviderFactory(_container);
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
    }
}