using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sprite.Data.Entities;
using Sprite.Data.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionRepositoryExtensions
    {
        public static void AddDefaultRepository(this IServiceCollection services, Type entityType, Type repositoryImpType)
        {
            var repositoryInterface = typeof(IRepository<>).MakeGenericType(entityType);
            if (repositoryInterface.IsAssignableFrom(repositoryImpType))
            {
                services.TryAddTransient(repositoryInterface, repositoryImpType);
            }

            var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);
            if (primaryKeyType != null)
            {
                var repositoryWithPkInterface = typeof(IRepository<,>).MakeGenericType(entityType, primaryKeyType);
                if (repositoryWithPkInterface.IsAssignableFrom(repositoryImpType))
                {
                    services.TryAddTransient(repositoryWithPkInterface, repositoryImpType);
                }
            }
        }
    }
}