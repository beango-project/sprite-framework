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
using Microsoft.EntityFrameworkCor;
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

        #region remove StereotypedSaveChangesInterceptor

        // public override int SaveChanges()
        // {
        //     foreach (var entityEntry in ChangeTracker.Entries())
        //     {
        //         switch (entityEntry.State)
        //         {
        //             case EntityState.Added:
        //                 DetectStereotypedForAdded(entityEntry);
        //                 break;
        //             case EntityState.Modified:
        //                 DetectStereotypedForModified(entityEntry);
        //                 break;
        //             case EntityState.Deleted:
        //                 break;
        //         }
        //     }
        //
        //     return base.SaveChanges();
        // }
        //
        //
        // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     foreach (var entityEntry in ChangeTracker.Entries())
        //     {
        //         DetectAndSetStereotypedCache(entityEntry);
        //         switch (entityEntry.State)
        //         {
        //             case EntityState.Added:
        //                 DetectStereotypedForAdded(entityEntry);
        //                 break;
        //             case EntityState.Modified:
        //                 DetectStereotypedForModified(entityEntry);
        //                 break;
        //             case EntityState.Deleted:
        //                 break;
        //         }
        //     }
        //
        //     return await base.SaveChangesAsync(cancellationToken);
        // }

        //
        // protected virtual void DetectAndSetStereotypedCache(EntityEntry entityEntry)
        // {
        //     var entityType = entityEntry.GetType();
        //     var propertyInfosCache = EntityChangePropertiesCache.Get(entityType);
        //     if (propertyInfosCache != null)
        //     {
        //         return;
        //     }
        //
        //
        //     var propertyEntries = entityEntry.Properties.Where(entry => entry.Metadata.PropertyInfo != null &&
        //                                                                 entry.Metadata.PropertyInfo.IsDefined(typeof(CreatedDateAttribute), true) ||
        //                                                                 entry.Metadata.PropertyInfo.IsDefined(typeof(CreateByAttribute), true) ||
        //                                                                 entry.Metadata.PropertyInfo.IsDefined(typeof(LastModifiedDateAttribute), true) ||
        //                                                                 entry.Metadata.PropertyInfo.IsDefined(typeof(LastModifiedByAttribute), true) ||
        //                                                                 entry.Metadata.PropertyInfo.IsDefined(typeof(DeleteTimeAttribute), true) ||
        //                                                                 entry.Metadata.PropertyInfo.IsDefined(typeof(DeleteByAttribute), true))
        //         .Select(entry => entry.Metadata.PropertyInfo);
        //     EntityChangePropertiesCache.Add(entityType, propertyEntries.ToArray());
        // }
        //
        // protected virtual void DetectStereotypedForAdded(EntityEntry entityEntry)
        // {
        //     foreach (var propertyInfo in EntityChangePropertiesCache.Get(entityEntry.GetType()))
        //     {
        //         if (propertyInfo.IsDefined(typeof(CreatedDateAttribute), true))
        //         {
        //             _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
        //             // ActivePropInfos.Add(propertyInfo);
        //         }
        //
        //         if (propertyInfo.IsDefined(typeof(CreateByAttribute), true))
        //         {
        //             var currentUser = PrincipalAccessor.CurrentPrincipal.GetCurrentUser();
        //             if (currentUser != null)
        //             {
        //                 var changeType = Convert.ChangeType(currentUser.Value, propertyInfo.PropertyType);
        //                 propertyInfo.SetValue(entityEntry.Entity, changeType);
        //             }
        //             // new PropertySetter(entityEntry.Entity, currentUser?.Value, propertyInfo);
        //             // ActivePropInfos.Add(propertyInfo);
        //         }
        //     }
        //
        //
        //     DetectIsCanBeSetId(entityEntry);
        // }
        //
        // protected virtual void DetectIsCanBeSetId(EntityEntry entry)
        // {
        //     var uidGeneratorExtension = _options.FindExtension<DbContextOptionsUidGeneratorExtension>();
        //     if (uidGeneratorExtension is null)
        //     {
        //         return;
        //     }
        //     //
        //     // if (uidGeneratorExtension.Options.IsDistributed! && entry.Entity is IEntity<Guid> entityWithGuidId)
        //     // {
        //     //     TryUniqueId(uidGeneratorExtension, entry);
        //     // }
        //
        //     TrySetDistributedUniqueId(uidGeneratorExtension, entry);
        // }
        //
        //
        // protected virtual void DetectStereotypedForModified(EntityEntry entityEntry)
        // {
        //     // 对于没有继承或者实现有IHasCreationTime接口的类型，才进行特性扫描
        //
        //     foreach (var propertyInfo in entityEntry.Entity.GetType().GetProperties())
        //     {
        //         if (propertyInfo.IsDefined(typeof(LastModifiedDateAttribute), true))
        //         {
        //             _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
        //         }
        //     }
        // }
        //
        // protected virtual void TryUniqueId(DbContextOptionsUidGeneratorExtension extension, EntityEntry entry)
        // {
        // }
        //
        // protected virtual void TrySetDistributedUniqueId(DbContextOptionsUidGeneratorExtension extension, EntityEntry entry)
        // {
        //     if (extension.Options.IsDistributed && entry.Entity is IEntity<long> entity)
        //     {
        //         if (entity.Id != default)
        //         {
        //             return;
        //         }
        //
        //         var idProp = entry.Property("Id").Metadata.PropertyInfo;
        //
        //
        //         var nextId = DistributedIdGenerator?.Value.NextId();
        //         if (nextId.HasValue)
        //         {
        //             new PropertySetter<long>(entity, nextId.Value, idProp);
        //         }
        //     }
        //     else
        //     {
        //         //TODO:如果不继承是否也设置ID？？
        //         // var propertyInfo = entry.Entity.GetType().GetProperties().SingleOrDefault(p => p.Name == "Id" && p.CanWrite);
        //         //
        //         // var propertyReaderWriter = new PropertyReaderWriter<long>(entry.Entity, propertyInfo);
        //         //
        //         // if (id == null)
        //         // {
        //         //     var nextId = DistributedIdGenerator?.Value.NextId();
        //         //     if (nextId.HasValue)
        //         //     {
        //         //         id = nextId.Value;
        //         //     }
        //         // }
        //     }
        // }

        #endregion


        public void UseTransaction(DbTransaction transaction)
        {
            DbContext.Database.UseTransactionAsync(transaction);
        }
    }
}