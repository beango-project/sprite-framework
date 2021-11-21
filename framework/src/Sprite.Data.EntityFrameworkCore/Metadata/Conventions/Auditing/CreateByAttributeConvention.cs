using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Sprite.Data.Entities.Auditing;

namespace Sprite.Data.EntityFrameworkCore.Conventions
{
    public class CreateByAttributeConvention : PropertyAttributeConventionBase<CreateByAttribute>
    {
        public CreateByAttributeConvention([NotNull] ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
        {
        }

        protected override void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, CreateByAttribute attribute, MemberInfo clrMember, IConventionContext context)
        {
            propertyBuilder.ValueGenerated(ValueGenerated.OnAdd, fromDataAnnotation: true);
        }
    }
}