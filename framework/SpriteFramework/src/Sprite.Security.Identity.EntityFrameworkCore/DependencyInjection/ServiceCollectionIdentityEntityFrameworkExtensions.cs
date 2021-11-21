using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Data.EntityFrameworkCore;

namespace Sprite.Security.Identity.EntityFrameworkCore.DependencyInjection
{
    public static class ServiceCollectionIdentityEntityFrameworkExtensions
    {
        public static IdentityBuilder AddIdentityEntityFrameworkStores<TContext>(this IdentityBuilder builder)
            where TContext : DbContextBase<TContext>
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TContext));
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type contextType)
        {
            var dbSetsTypes = contextType.GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GenericTypeArguments[0]);

            Dictionary<Type, KeyValuePair<Type, Type>> matchTypes = new Dictionary<Type, KeyValuePair<Type, Type>>();

            matchTypes.Add(typeof(IdentityUser<>), new KeyValuePair<Type, Type>());
            matchTypes.Add(typeof(IdentityUserClaim<,>), new KeyValuePair<Type, Type>());
            matchTypes.Add(typeof(IdentityUserRole<,>), new KeyValuePair<Type, Type>());
            matchTypes.Add(typeof(IdentityUserToken<,>), new KeyValuePair<Type, Type>());
            matchTypes.Add(typeof(IdentityUserLogin<,>), new KeyValuePair<Type, Type>());
            matchTypes.Add(typeof(IdentityRole<>), new KeyValuePair<Type, Type>());
            matchTypes.Add(typeof(IdentityRoleClaim<,>), new KeyValuePair<Type, Type>());


            foreach (var dbSetsType in dbSetsTypes)
            {
                foreach (var (k, v) in matchTypes)
                {
                    if (v.Key == null)
                    {
                        var matchType = TypeHelper.GetMatchType2(dbSetsType, k);
                        if (matchType != null)
                        {
                            matchTypes[k] = new KeyValuePair<Type, Type>(dbSetsType, matchType);
                        }
                    }
                }
            }

            //user
            var identityUserStore = typeof(IdentityUserStore<,,,,,,,,,,,,>).MakeGenericType(
                matchTypes[typeof(IdentityUser<>)].Key,
                matchTypes[typeof(IdentityUser<>)].Value.GenericTypeArguments[0],
                matchTypes[typeof(IdentityRole<>)].Key,
                matchTypes[typeof(IdentityRole<>)].Value.GenericTypeArguments[0],
                matchTypes[typeof(IdentityUserRole<,>)].Key,
                matchTypes[typeof(IdentityUserLogin<,>)].Key,
                matchTypes[typeof(IdentityUserLogin<,>)].Value.GenericTypeArguments[0],
                matchTypes[typeof(IdentityUserToken<,>)].Key,
                matchTypes[typeof(IdentityUserToken<,>)].Value.GenericTypeArguments[0],
                matchTypes[typeof(IdentityUserClaim<,>)].Key,
                matchTypes[typeof(IdentityUserClaim<,>)].Value.GenericTypeArguments[0],
                matchTypes[typeof(IdentityRoleClaim<,>)].Key,
                matchTypes[typeof(IdentityRoleClaim<,>)].Value.GenericTypeArguments[0]
            );
            services.AddScoped(typeof(IUserStore<>), identityUserStore);

            //role
            var identityRoleStore = typeof(IdentityRoleStore<,,,,,>).MakeGenericType(
                matchTypes[typeof(IdentityUser<>)].Key,
                matchTypes[typeof(IdentityUser<>)].Value.GenericTypeArguments[0],
                matchTypes[typeof(IdentityRole<>)].Key,
                matchTypes[typeof(IdentityRole<>)].Value.GenericTypeArguments[0],
                matchTypes[typeof(IdentityRoleClaim<,>)].Key,
                matchTypes[typeof(IdentityRoleClaim<,>)].Value.GenericTypeArguments[0]
            );
            services.AddScoped(typeof(IRoleStore<>), identityRoleStore);

            // Dictionary<Type, KeyValuePair<Type, Type>> matchTypes = new Dictionary<Type, KeyValuePair<Type, Type>>();
            // matchTypes.Add(typeof(IdentityUser<>), new KeyValuePair<Type, Type>());
            // matchTypes.Add(typeof(IdentityUserClaim<,>), new KeyValuePair<Type, Type>());
            // matchTypes.Add(typeof(IdentityUserRole<,,>), new KeyValuePair<Type, Type>());
            // matchTypes.Add(typeof(IdentityUserToken<,>), new KeyValuePair<Type, Type>());
            // matchTypes.Add(typeof(IdentityUserLogin<,>), new KeyValuePair<Type, Type>());
            // matchTypes.Add(typeof(IdentityRole<>), new KeyValuePair<Type, Type>());
            // matchTypes.Add(typeof(IdentityRoleClaim<,>), new KeyValuePair<Type, Type>());
            // foreach (var dbSetsType in dbSetsTypes)
            // {
            //     foreach (var (k, v) in matchTypes)
            //     {
            //         if (v.Key == null)
            //         {
            //             var matchType = TypeHelper.GetMatchType(dbSetsType, k);
            //             if (matchType != null)
            //             {
            //                 matchTypes[k] = new KeyValuePair<Type, Type>(dbSetsType, matchType);
            //             }
            //         }
            //     }
            // }
            // //user
            // var identityUserStore = typeof(IdentityUserStore<,,,,,,,,,,,,,>).MakeGenericType(
            //     matchTypes[typeof(IdentityUser<>)].Key,
            //     matchTypes[typeof(IdentityUser<>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityRole<>)].Key,
            //     matchTypes[typeof(IdentityRole<>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityUserRole<,,>)].Key,
            //     matchTypes[typeof(IdentityUserRole<,,>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityUserLogin<,>)].Key,
            //     matchTypes[typeof(IdentityUserLogin<,>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityUserToken<,>)].Key,
            //     matchTypes[typeof(IdentityUserToken<,>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityUserClaim<,>)].Key,
            //     matchTypes[typeof(IdentityUserClaim<,>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityRoleClaim<,>)].Key,
            //     matchTypes[typeof(IdentityRoleClaim<,>)].Value.GenericTypeArguments[0]
            // );
            // services.AddScoped(typeof(IUserStore<>), identityUserStore);
            //
            // //role
            // var identityRoleStore = typeof(IdentityRoleStore<,,,,,>).MakeGenericType(
            //     matchTypes[typeof(IdentityUser<>)].Key,
            //     matchTypes[typeof(IdentityUser<>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityRole<>)].Key,
            //     matchTypes[typeof(IdentityRole<>)].Value.GenericTypeArguments[0],
            //     matchTypes[typeof(IdentityRoleClaim<,>)].Key,
            //     matchTypes[typeof(IdentityRoleClaim<,>)].Value.GenericTypeArguments[0]
            // );
            // services.AddScoped(typeof(IRoleStore<>), identityRoleStore);
        }
    }
}