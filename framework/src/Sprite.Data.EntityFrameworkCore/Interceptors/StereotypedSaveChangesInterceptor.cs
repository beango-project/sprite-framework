using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;
using ImmediateReflection;
using ImTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
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
        private DbContextUidGeneratorOptions? _uidGeneratorOptions => _options.FindExtension<DbContextOptionsUidGeneratorExtension>()?.Options;

        private IServiceProvider _serviceProvider;

        protected virtual object IdGenerator => _serviceProvider.GetService(_uidGeneratorOptions.UidGeneratorType);

        public StereotypedSaveChangesInterceptor(DbContextOptions<TDbContext> options, IServiceProvider serviceProvider)
        {
            _options = options;
            _serviceProvider = serviceProvider;
        }


        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken
            cancellationToken = default)
        {
            if (eventData.Context != null && eventData.Context.ChangeTracker.HasChanges())
            {
                await Task.Run(() =>
                {
                    foreach (var entityEntry in eventData.Context.ChangeTracker.Entries())
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
                                DetectStereotypedForDeleted(entityEntry);
                                break;
                        }
                    }
                }, cancellationToken);
            }


            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void DetectAndSetStereotypedCache(EntityEntry entityEntry)
        {
            var entityType = entityEntry.Entity.GetType();
            var propertyInfosCache = EntityChangePropertiesCache.Get(entityType);
            if (propertyInfosCache != null)
            {
                return;
            }

            BuildEntityChangePropertiesCache(entityEntry);
        }

        /// <summary>
        /// 构建实体需要更变属性的缓存，以便在后续调用直接从缓存中获取使用，而不必重新反射判断并获取
        /// </summary>
        /// <param name="entityEntry"></param>
        private void BuildEntityChangePropertiesCache(EntityEntry entityEntry)
        {
            var list = new List<EntityChangePropertyEntry>();

            foreach (var entityEntryProperty in entityEntry.Properties)
            {
                var propertyInfo = entityEntryProperty.Metadata.PropertyInfo;
                if (propertyInfo != null)
                {
                    if (propertyInfo.IsDefined<CreatedDateAttribute>(true))
                    {
                        list.Add(new EntityChangePropertyEntry(propertyInfo, typeof(CreatedDateAttribute)));
                    }
                    else if (propertyInfo.IsDefined<CreateByAttribute>(true))
                    {
                        list.Add(new EntityChangePropertyEntry(propertyInfo, typeof(CreateByAttribute)));
                    }
                    else if (propertyInfo.IsDefined<LastModifiedDateAttribute>(true))
                    {
                        list.Add(new EntityChangePropertyEntry(propertyInfo, typeof(LastModifiedDateAttribute)));
                    }
                    else if (propertyInfo.IsDefined<LastModifiedByAttribute>(true))
                    {
                        list.Add(new EntityChangePropertyEntry(propertyInfo, typeof(LastModifiedByAttribute)));
                    }
                    else if (propertyInfo.IsDefined<DeletionTimeAttribute>(true))
                    {
                        list.Add(new EntityChangePropertyEntry(propertyInfo, typeof(DeletionTimeAttribute)));
                    }
                    else if (propertyInfo.IsDefined<DeleteByAttribute>(true))
                    {
                        list.Add(new EntityChangePropertyEntry(propertyInfo, typeof(DeleteByAttribute)));
                    }
                }
            }

            EntityChangePropertiesCache.Add(entityEntry.Entity.GetType(), list.ToArray());
        }

        private void DetectStereotypedForAdded(EntityEntry entityEntry)
        {
            EntityChangePropertiesCache.Get(entityEntry.Entity.GetType()).ForEach(propertyEntry =>
            {
                if (propertyEntry.IsCandidate(typeof(CreatedDateAttribute)))
                {
                    propertyEntry.SetValue(entityEntry.Entity, DateTime.Now);
                }

                if (propertyEntry.IsCandidate(typeof(CreateByAttribute)))
                {
                    // var currentUser = PrincipalAccessor.CurrentPrincipal.GetCurrentUser();
                    var currentUser = _serviceProvider.GetService<ICurrentPrincipalAccessor>().CurrentPrincipal.GetCurrentUser();
                    if (currentUser != null)
                    {
                        var changeType = Convert.ChangeType(currentUser.Value, propertyEntry.GetProperty().GetType());
                        propertyEntry.SetValue(entityEntry.Entity, changeType);
                    }
                }
            });

            // foreach (var propertyInfo in EntityChangePropertiesCache.Get(entityEntry.GetType()))
            // {
            //     if (propertyInfo.IsDefined(typeof(CreatedDateAttribute), true))
            //     {
            //         _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
            //         // ActivePropInfos.Add(propertyInfo);
            //     }
            //
            //     if (propertyInfo.IsDefined(typeof(CreateByAttribute), true))
            //     {
            //         var currentUser = PrincipalAccessor.CurrentPrincipal.GetCurrentUser();
            //         if (currentUser != null)
            //         {
            //             var changeType = Convert.ChangeType(currentUser.Value, propertyInfo.PropertyType);
            //             propertyInfo.SetValue(entityEntry.Entity, changeType);
            //         }
            //         // new PropertySetter(entityEntry.Entity, currentUser?.Value, propertyInfo);
            //         // ActivePropInfos.Add(propertyInfo);
            //     }
            //     //TODO 添加定义创建人特性的属性 
            // }


            DetectIsCanBeSetId(entityEntry);
        }

        private void DetectIsCanBeSetId(EntityEntry entry)
        {
            var options = _uidGeneratorOptions;

            if (options is null)
            {
                return;
            }

            //
            if (entry.Entity is IEntity<Guid> && IdGenerator is IUniqueIdGenerator uniqueIdGenerator)
            {
                TrySetUniqueId(uniqueIdGenerator, entry);
                return;
            }

            if (IdGenerator is IDistributedUniqueIdGenerator distributedUniqueIdGenerator)
            {
                TrySetDistributedUniqueId(distributedUniqueIdGenerator, entry);
            }
        }


        private void DetectStereotypedForModified(EntityEntry entityEntry)
        {
            EntityChangePropertiesCache.Get(entityEntry.Entity.GetType()).ForEach(propertyEntry =>
            {
                if (propertyEntry.IsCandidate(typeof(LastModifiedDateAttribute)))
                {
                    propertyEntry.SetValue(entityEntry.Entity, DateTime.Now);
                }

                if (propertyEntry.IsCandidate(typeof(LastModifiedByAttribute)))
                {
                    var currentUser = _serviceProvider.GetService<ICurrentPrincipalAccessor>().CurrentPrincipal.GetCurrentUser();
                    if (currentUser != null)
                    {
                        var changeType = Convert.ChangeType(currentUser.Value, propertyEntry.GetProperty().GetType());
                        propertyEntry.SetValue(entityEntry.Entity, changeType);
                    }
                }
            });

            // foreach (var propertyInfo in EntityChangePropertiesCache.Get(entityEntry.GetType()))
            // {
            //     if (propertyInfo.IsDefined(typeof(LastModifiedDateAttribute), true))
            //     {
            //         _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
            //     }
            //
            //     if (propertyInfo.IsDefined(typeof(LastModifiedByAttribute), true))
            //     {
            //         var currentUser = PrincipalAccessor.CurrentPrincipal.GetCurrentUser();
            //         if (currentUser != null)
            //         {
            //             var changeType = Convert.ChangeType(currentUser.Value, propertyInfo.PropertyType);
            //             propertyInfo.SetValue(entityEntry.Entity, changeType);
            //         }
            //     }
            // }
        }

        private void DetectStereotypedForDeleted(EntityEntry entityEntry)
        {
            //set soft delete 
            if (entityEntry.Entity is ISoftDelete softDelete)
            {
                entityEntry.State = EntityState.Modified;
                softDelete.IsDeleted = true;
            }

            EntityChangePropertiesCache.Get(entityEntry.Entity.GetType()).ForEach(propertyEntry =>
            {
                if (propertyEntry.IsCandidate(typeof(DeletionTimeAttribute)))
                {
                    propertyEntry.SetValue(entityEntry.Entity, DateTime.Now);
                }

                if (propertyEntry.IsCandidate(typeof(DeleteByAttribute)))
                {
                    var currentUser = _serviceProvider.GetService<ICurrentPrincipalAccessor>().CurrentPrincipal.GetCurrentUser();
                    if (currentUser != null)
                    {
                        var changeType = Convert.ChangeType(currentUser.Value, propertyEntry.GetProperty().GetType());
                        propertyEntry.SetValue(entityEntry.Entity, changeType);
                    }
                }
            });

            // foreach (var propertyInfo in EntityChangePropertiesCache.Get(entityEntry.GetType()))
            // {
            //     if (propertyInfo.IsDefined(typeof(DeleteTimeAttribute), true))
            //     {
            //         _ = new PropertySetter<DateTime>(entityEntry.Entity, DateTime.Now, propertyInfo);
            //     }
            //
            //     if (propertyInfo.IsDefined(typeof(DeleteByAttribute), true))
            //     {
            //         var currentUser = PrincipalAccessor.CurrentPrincipal.GetCurrentUser();
            //         if (currentUser != null)
            //         {
            //             var changeType = Convert.ChangeType(currentUser.Value, propertyInfo.PropertyType);
            //             propertyInfo.SetValue(entityEntry.Entity, changeType);
            //         }
            //     }
            // }
        }

        private void TrySetUniqueId(IUniqueIdGenerator idGenerator, EntityEntry entry)
        {
        }

        private void TrySetDistributedUniqueId(IDistributedUniqueIdGenerator idGenerator, EntityEntry entry)
        {
            //使用的ID生成器类型必须和


            if (entry.Entity is IEntity<long> longEntity)
            {
                if (longEntity.Id != default)
                {
                    return;
                }

                var idProp = entry.Property("Id").Metadata.PropertyInfo;


                var nextId = idGenerator.NextId();
                if (nextId != null)
                {
                    new PropertySetter<long>(longEntity, nextId, idProp);
                }
            }

            if (entry.Entity is IEntity<ulong> ulongEntity)
            {
                if (ulongEntity.Id != default)
                {
                    return;
                }

                var idProp = entry.Property("Id").Metadata.PropertyInfo;


                var nextId = idGenerator.NextId();
                if (nextId != null)
                {
                    new PropertySetter<ulong>(ulongEntity, (ulong)nextId, idProp);
                }
            }
            // else
            // {
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
            // }
        }
    }
}