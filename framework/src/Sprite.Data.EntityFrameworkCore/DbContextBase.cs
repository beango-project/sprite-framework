using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Sprite.Data.Entities;
using Sprite.Data.Entities.Auditing;
using Sprite.Data.EntityFrameworkCore.Extensions;
using Sprite.Data.EntityFrameworkCore.Interceptors;
using Sprite.DependencyInjection.Attributes;
using Sprite.UidGenerator;
using Z.EntityFramework.Plus;

namespace Sprite.Data.EntityFrameworkCore
{
    public class DbContextBase<TDbContext> : DbContext
        where TDbContext : DbContext
    {
        private readonly DbContextOptions<TDbContext> _options;

        protected internal IServiceProvider ServiceProvider { get; set; }

        public DbContextBase(DbContextOptions<TDbContext> options)
            : base(options)
        {
            _options = options;
        }


        protected virtual TDbContext DbContext { get; }

        public IDbContextTransaction? DbContextTransaction => DbContext.Database.CurrentTransaction;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // DbContextOptionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.AddInterceptors(new StereotypedSaveChangesInterceptor<TDbContext>(_options, ServiceProvider));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureBasePropertiesMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
                //
                // ConfigureValueConverterMethodInfo
                //     .MakeGenericMethod(entityType.ClrType)
                //     .Invoke(this, new object[] { modelBuilder, entityType });


                ConfigureValueGeneratedMethodInfo.GetMemberInfo()
                    .MakeGenericMethod(entityType.ClrType).GetReflector()
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }


        private static readonly MethodInfo ConfigureBasePropertiesMethodInfo
            = typeof(DbContextBase<TDbContext>)
                .GetMethod(
                    nameof(ConfigureBaseProperties),
                    BindingFlags.Instance | BindingFlags.NonPublic
                );
        //
        // private static readonly MethodInfo ConfigureValueConverterMethodInfo
        //     = typeof(DbContextBase<TDbContext>)
        //         .GetMethod(
        //             nameof(ConfigureValueConverter),
        //             BindingFlags.Instance | BindingFlags.NonPublic
        //         );

        private static readonly MethodReflector ConfigureValueGeneratedMethodInfo
            = typeof(DbContextBase<TDbContext>)
                .GetMethod(
                    nameof(ConfigureValueGenerated),
                    BindingFlags.Instance | BindingFlags.NonPublic
                ).GetReflector();


        protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (mutableEntityType.IsOwned())
            {
                return;
            }

            if (!typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
            {
                return;
            }

            ConfigureGlobalFilters<TEntity>(modelBuilder, mutableEntityType);
        }

        protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (mutableEntityType.BaseType == null && typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }

        private LambdaExpression CreateFilterExpression<TEntity>() where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                expression = e => !EF.Property<bool>(e, "IsDeleted");
            }

            return expression;
        }

        protected virtual void ConfigureValueGenerated<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (typeof(IEntity<Guid>).IsAssignableFrom(typeof(TEntity)))
            {
                var idPropertyBuilder = modelBuilder.Entity<TEntity>().Property(x => ((IEntity<Guid>)x).Id);
                if (idPropertyBuilder.Metadata.PropertyInfo.IsDefinedAttribute(typeof(DatabaseGeneratedAttribute), true))
                {
                    return;
                }

                idPropertyBuilder.ValueGeneratedOnAdd();
            }

            if (typeof(IEntity<long>).IsAssignableFrom(typeof(TEntity)))
            {
                var idPropertyBuilder = modelBuilder.Entity<TEntity>().Property(x => ((IEntity<long>)x).Id);
                if (idPropertyBuilder.Metadata.PropertyInfo.IsDefinedAttribute(typeof(DatabaseGeneratedAttribute), true))
                {
                    return;
                }

                idPropertyBuilder.ValueGeneratedOnAdd();
            }

            if (typeof(IEntity<ulong>).IsAssignableFrom(typeof(TEntity)))
            {
                var idPropertyBuilder = modelBuilder.Entity<TEntity>().Property(x => ((IEntity<ulong>)x).Id);
                if (idPropertyBuilder.Metadata.PropertyInfo.IsDefinedAttribute(typeof(DatabaseGeneratedAttribute), true))
                {
                    return;
                }

                idPropertyBuilder.ValueGeneratedOnAdd();
            }
        }
        

        public void UseTransaction(DbTransaction transaction)
        {
            DbContext.Database.UseTransactionAsync(transaction);
        }
    }
}