using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sprite.Data.Entities;
using Sprite.Data.Entities.Auditing;
using Sprite.Data.EntityFrameworkCore.Extensions;
using Sprite.UidGenerator;

namespace Sprite.Data.EntityFrameworkCore.Interceptors
{
    public class StereotypedSaveChangesInterceptor<TDbContext> : SaveChangesInterceptor
        where TDbContext : DbContext
    {
        private readonly DbContextOptions<TDbContext> _options;

        public StereotypedSaveChangesInterceptor(DbContextOptions<TDbContext> options, Lazy<ICurrentPrincipalAccessor> principalAccessor, Lazy<IUniqueIdGenerator> idGenerator,
            Lazy<IDistributedUniqueIdGenerator> distributedIdGenerator)
        {
            _options = options;
            _principalAccessor = principalAccessor;
            _idGenerator = idGenerator;
            _distributedIdGenerator = distributedIdGenerator;
        }


        private readonly Lazy<ICurrentPrincipalAccessor> _principalAccessor;

        protected virtual ICurrentPrincipalAccessor PrincipalAccessor => _principalAccessor.Value;


        private readonly Lazy<IUniqueIdGenerator> _idGenerator;

        private readonly Lazy<IDistributedUniqueIdGenerator> _distributedIdGenerator;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken
            cancellationToken = default)
        {
            foreach (var entityEntry in eventData.Context?.ChangeTracker.Entries())
            {
                DetectAndSetStereotypedCache(entityEntry);
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        DetectStereotypedForAdded(entityEntry);
                        break;
                    case EntityState.Modified:
                        DetectStereotypedForModified(entityEntry);
                        break;
                    case EntityState.Deleted:
                        break;
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void DetectAndSetStereotypedCache(EntityEntry entityEntry)
        {
            var entityType = entityEntry.GetType();
            var propertyInfosCache = EntityChangePropertiesCache.Get(entityType);
            if (propertyInfosCache != null)
            {
                return;
            }


            var propertyEntries = entityEntry.Properties.Where(entry => entry.Metadata.PropertyInfo != null &&
                                                                        entry.Metadata.PropertyInfo.IsDefined(typeof(CreatedDateAttribute), true) ||
                                                                        entry.Metadata.PropertyInfo.IsDefined(typeof(CreateByAttribute), true) ||
                                                                        entry.Metadata.PropertyInfo.IsDefined(typeof(LastModifiedDateAttribute), true) ||
                                                                        entry.Metadata.PropertyInfo.IsDefined(typeof(LastModifiedByAttribute), true) ||
                                                                        entry.Metadata.PropertyInfo.IsDefined(typeof(DeleteTimeAttribute), true) ||
                                                                        entry.Metadata.PropertyInfo.IsDefined(typeof(DeleteByAttribute), true))
                .Select(entry => entry.Metadata.PropertyInfo);
            EntityChangePropertiesCache.Add(entityType, propertyEntries.ToArray());
        }

        private void DetectStereotypedForAdded(EntityEntry entityEntry)
        {
            foreach (var propertyInfo in EntityChangePropertiesCache.Get(entityEntry.GetType()))
            {
                if (propertyInfo.IsDefined(typeof(CreatedDateAttribute), true))
                {
                    _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
                    // ActivePropInfos.Add(propertyInfo);
                }

                if (propertyInfo.IsDefined(typeof(CreateByAttribute), true))
                {
                    var currentUser = PrincipalAccessor.CurrentPrincipal.GetCurrentUser();
                    if (currentUser != null)
                    {
                        var changeType = Convert.ChangeType(currentUser.Value, propertyInfo.PropertyType);
                        propertyInfo.SetValue(entityEntry.Entity, changeType);
                    }
                    // new PropertySetter(entityEntry.Entity, currentUser?.Value, propertyInfo);
                    // ActivePropInfos.Add(propertyInfo);
                }
                //TODO 添加定义创建人特性的属性 
            }

            // propertyInfos = ActivePropInfos.ToArray();
            // EntityChangePropertiesCache.Add(entityType, propertyInfos);
            // }

            // foreach (var propertyInfo in entityEntry.Entity.GetType().GetProperties())
            // {
            //     if (propertyInfo.IsDefined(typeof(CreatedDateAttribute), true))
            //     {
            //         _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
            //     }
            // }

            DetectIsCanBeSetId(entityEntry);
        }

        private void DetectIsCanBeSetId(EntityEntry entry)
        {
            var uidGeneratorExtension = _options.FindExtension<DbContextOptionsUidGeneratorExtension>();
            if (uidGeneratorExtension is null)
            {
                return;
            }
            //
            // if (uidGeneratorExtension.Options.IsDistributed! && entry.Entity is IEntity<Guid> entityWithGuidId)
            // {
            //     TryUniqueId(uidGeneratorExtension, entry);
            // }

            TrySetDistributedUniqueId(uidGeneratorExtension, entry);
        }


        private void DetectStereotypedForModified(EntityEntry entityEntry)
        {
            // 对于没有继承或者实现有IHasCreationTime接口的类型，才进行特性扫描

            foreach (var propertyInfo in entityEntry.Entity.GetType().GetProperties())
            {
                if (propertyInfo.IsDefined(typeof(LastModifiedDateAttribute), true))
                {
                    _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
                }
            }
        }

        private void TryUniqueId(DbContextOptionsUidGeneratorExtension extension, EntityEntry entry)
        {
        }

        private void TrySetDistributedUniqueId(DbContextOptionsUidGeneratorExtension extension, EntityEntry entry)
        {
            if (extension.Options.IsDistributed)
            {
                if (entry.Entity is IEntity<long> longEntity)
                {
                    if (longEntity.Id != default)
                    {
                        return;
                    }

                    var idProp = entry.Property("Id").Metadata.PropertyInfo;


                    var nextId = _distributedIdGenerator?.Value.NextId();
                    if (nextId.HasValue)
                    {
                        new PropertySetter<long>(longEntity, nextId.Value, idProp);
                    }
                }

                if (entry.Entity is IEntity<ulong> ulongEntity)
                {
                    if (ulongEntity.Id != default)
                    {
                        return;
                    }

                    var idProp = entry.Property("Id").Metadata.PropertyInfo;


                    var nextId = _distributedIdGenerator?.Value.NextId();
                    if (nextId.HasValue)
                    {
                        new PropertySetter<ulong>(ulongEntity, (ulong)nextId.Value, idProp);
                    }
                }
            }
            else
            {
                //TODO:如果不继承是否也设置ID？？
                // var propertyInfo = entry.Entity.GetType().GetProperties().SingleOrDefault(p => p.Name == "Id" && p.CanWrite);
                //
                // var propertyReaderWriter = new PropertyReaderWriter<long>(entry.Entity, propertyInfo);
                //
                // if (id == null)
                // {
                //     var nextId = DistributedIdGenerator?.Value.NextId();
                //     if (nextId.HasValue)
                //     {
                //         id = nextId.Value;
                //     }
                // }
            }
        }
    }
}