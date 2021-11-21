using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sprite.Data.DependencyInjection;
using Sprite.Data.Entities;
using Sprite.Data.Repositories;

namespace Sprite.Data.EntityFrameworkCore.Repositories
{
    public class EfCoreRepositoryRegistrar : RepositoryRegistrar<PersistenceVenderRegistrationOptions>
    {
        public EfCoreRepositoryRegistrar(PersistenceVenderRegistrationOptions options) : base(options)
        {
        }

        protected override IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            // return dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //     .Where(x => ReflectionHelper.IsAssignableToGenericType(x.GetType().GetTypeInfo(), typeof(DbSet<>))
            //                 && typeof(IEntity).IsAssignableFrom(x.PropertyType.GenericTypeArguments[0]))
            //     .Select(x => x.PropertyType.GenericTypeArguments[0]);

            var types = from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property.PropertyType.GenericTypeArguments[0];

            return types;
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType)
        {
            return typeof(EfCoreRepository<,>).MakeGenericType(dbContextType, entityType);
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
        {
            return typeof(EfCoreRepository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
        }
    }
}