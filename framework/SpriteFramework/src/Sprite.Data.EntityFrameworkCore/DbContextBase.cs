using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
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

namespace Sprite.Data.EntityFrameworkCore
{
    // internal class ChangeAudit
    // {
    //     public ChangeAudit(EntityEntry entityEntry, DateTime changedTime)
    //     {
    //         EntityEntry = entityEntry;
    //         ChangedTime = changedTime;
    //     }
    //
    //     public EntityEntry EntityEntry { get; set; }
    //
    //     public DateTime ChangedTime { get; set; }
    // }

    public class DbContextBase<TDbContext> : DbContext
        where TDbContext : DbContext
    {
        private readonly DbContextOptions<TDbContext> _options;

        // private readonly List<ChangeAudit> _audits = new List<ChangeAudit>();


        public DbContextBase(DbContextOptions<TDbContext> options)
            : base(options)
        {
            _options = options;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // DbContextOptionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.AddInterceptors(new[] { new StereotypedSaveChangesInterceptor<TDbContext>(_options, _principalAccessor, _idGenerator, _distributedIdGenerator) });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // ConfigureBasePropertiesMethodInfo
                //     .MakeGenericMethod(entityType.ClrType)
                //     .Invoke(this, new object[] { modelBuilder, entityType });
                //
                // ConfigureValueConverterMethodInfo
                //     .MakeGenericMethod(entityType.ClrType)
                //     .Invoke(this, new object[] { modelBuilder, entityType });

                ConfigureValueGeneratedMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        protected virtual TDbContext DbContext { get; }

        [Autowired]
        private Lazy<ICurrentPrincipalAccessor> _principalAccessor;

        [Autowired]
        private Lazy<IUniqueIdGenerator> _idGenerator;

        [Autowired]
        private Lazy<IDistributedUniqueIdGenerator> _distributedIdGenerator;

        protected virtual ICurrentPrincipalAccessor? PrincipalAccessor => _principalAccessor.Value;


        public IDbContextTransaction? DbContextTransaction => DbContext.Database.CurrentTransaction;


        protected virtual IUniqueIdGenerator IdGenerator => _idGenerator.Value;


        protected virtual IDistributedUniqueIdGenerator? DistributedIdGenerator => _distributedIdGenerator.Value;

        // private static readonly MethodInfo ConfigureBasePropertiesMethodInfo
        //     = typeof(DbContextBase<TDbContext>)
        //         .GetMethod(
        //             nameof(ConfigureBaseProperties),
        //             BindingFlags.Instance | BindingFlags.NonPublic
        //         );
        //
        // private static readonly MethodInfo ConfigureValueConverterMethodInfo
        //     = typeof(DbContextBase<TDbContext>)
        //         .GetMethod(
        //             nameof(ConfigureValueConverter),
        //             BindingFlags.Instance | BindingFlags.NonPublic
        //         );

        private static readonly MethodInfo ConfigureValueGeneratedMethodInfo
            = typeof(DbContextBase<TDbContext>)
                .GetMethod(
                    nameof(ConfigureValueGenerated),
                    BindingFlags.Instance | BindingFlags.NonPublic
                );


        protected virtual void ConfigureValueGenerated<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (typeof(IEntity<Guid>).IsAssignableFrom(typeof(TEntity)))
            {
                var idPropertyBuilder = modelBuilder.Entity<TEntity>().Property(x => ((IEntity<Guid>)x).Id);
                if (idPropertyBuilder.Metadata.PropertyInfo.IsDefined(typeof(DatabaseGeneratedAttribute), true))
                {
                    return;
                }

                idPropertyBuilder.ValueGeneratedOnAdd();
            }

            if (typeof(IEntity<long>).IsAssignableFrom(typeof(TEntity)))
            {
                var idPropertyBuilder = modelBuilder.Entity<TEntity>().Property(x => ((IEntity<long>)x).Id);
                if (idPropertyBuilder.Metadata.PropertyInfo.IsDefined(typeof(DatabaseGeneratedAttribute), true))
                {
                    return;
                }

                idPropertyBuilder.ValueGeneratedOnAdd();
            }

            if (typeof(IEntity<ulong>).IsAssignableFrom(typeof(TEntity)))
            {
                var idPropertyBuilder = modelBuilder.Entity<TEntity>().Property(x => ((IEntity<ulong>)x).Id);
                if (idPropertyBuilder.Metadata.PropertyInfo.IsDefined(typeof(DatabaseGeneratedAttribute), true))
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

        // public override void Dispose()
        // {
        //     Console.WriteLine("DbContext已经销毁");
        //     base.Dispose();
        // }
    }
}