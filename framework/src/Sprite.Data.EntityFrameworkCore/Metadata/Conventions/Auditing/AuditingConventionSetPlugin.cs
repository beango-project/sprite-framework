using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Sprite.Data.EntityFrameworkCore.Conventions
{
    public class AuditingConventionSetPlugin : IConventionSetPlugin
    {
        public ConventionSet ModifyConventions(ConventionSet conventionSet)
        {
            conventionSet.PropertyAddedConventions.Add(new CreateByAttributeConvention(null));
            return conventionSet;
        }
    }
}