using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sprite.Data.Entities;
using Sprite.Data.Uow;

namespace Sprite.Data.Repositories
{
    public interface IRepository
    {
    }

    public interface IRepository<TEntity> : IRepository, IQueryable<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// 根据表达式获取实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>实体</returns>
        TEntity Get([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns>全部实体结果集</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 获取全部，可提取导航属性
        /// </summary>
        /// <returns>全部实体结果集</returns>
        IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] propertySelectors);


        List<TEntity> GetList();

        List<TEntity> GetList(params Expression<Func<TEntity, object>>[] propertySelectors);

        Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors);


        /// <summary>
        /// 第一个是判断的条件，第二个提取本身的对象数据集合外的数据
        /// </summary>
        /// <param name="includeProperties">需要直接提取关联类集合数据的表达式集合，通过逗号隔开</param>
        IQueryable<TEntity> GetAllMerge(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);

        /// <summary>
        /// 第一个是判断的条件，第二个提取本身的对象数据集合外的数据
        /// </summary>
        /// <param name="includeProperties">需要直接提取关联类集合数据的表达式集合，通过逗号隔开</param>
        Task<IQueryable<TEntity>> GetAllMergeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[]
                propertySelectors);

        /// <summary>
        /// Get a single entity by the given <paramref name="predicate" />.
        /// It throws <see cref="EntityNotFoundException" /> if there is no entity with the given <paramref name="predicate" />.
        /// It throws <see cref="InvalidOperationException" /> if there are multiple entities with the given <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <param name="includeDetails">Set true to include all children of this entity</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<TEntity> GetAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);


        Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors);
        


        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);


        Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);


        Task<TEntity> AddAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// Inserts multiple new entities.
        /// </summary>
        /// <param name="autoSave">
        /// Set true to automatically save changes to database.
        /// This is useful for ORMs / database APIs those only save changes with an explicit method call, but you need to immediately save changes to the database.
        /// </param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="entities">Entities to be inserted.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        Task AddManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 执行非查询更新
        /// </summary>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> UpdateAsync([NotNull] Expression<Func<TEntity, bool>> predicate, [NotNull] Expression<Func<TEntity, TEntity>> expression, bool autoSave = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates multiple entities.
        /// </summary>
        /// <param name="entities">Entities to be updated.</param>
        /// <param name="autoSave">
        /// Set true to automatically save changes to database.
        /// This is useful for ORMs / database APIs those only save changes with an explicit method call, but you need to immediately save changes to the database.
        /// </param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        Task UpdateManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        Task DeleteAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// An IQueryable&lt;T&gt; extension method that deletes all rows asynchronously from the query
        /// without retrieving entities.
        /// </summary>
        /// <typeparam name="T">The type of elements of the query.</typeparam>
        /// <param name="predicate"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="query">The query to delete rows from without retrieving entities.</param>
        /// <returns>A task with the number of rows affected.</returns>
        Task<int> DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default);

        Task<int> DeleteManyAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        /// <param name="entities">Entities to be deleted.</param>
        /// <param name="autoSave">
        /// Set true to automatically save changes to database.
        /// This is useful for ORMs / database APIs those only save changes with an explicit method call, but you need to immediately save changes to the database.
        /// </param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        Task DeleteManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据 Lambda 条件 predicate 获取一个单一对象，如果没有或者有多个，抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步方法，根据 Lambda 条件 predicate 获取一个单一对象，如果没有或者有多个，抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据 Lambda 条件 predicate 获取一个单一对象，如果没有则返回默认值，有多个则抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// 异步方法， 根据 Lambda 条件 predicate 获取一个单一对象，如果没有则返回默认值，有多个则抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        
        // Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
        where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 获取具有给定主键的实体。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(TKey id);

        TEntity Get(TKey id, params Expression<Func<TEntity, object>>[] propertySelectors);

        /// <summary>
        /// Gets an entity with given primary key.
        /// Throws <see cref="EntityNotFoundException" /> if can not find an entity with given id.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="includeDetails">Set true to include all children of this entity</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Entity</returns>
        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors);

        TEntity Find(TKey id);

        /// <summary>
        /// Gets an entity with given primary key or null if not found.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="includeDetails">Set true to include all children of this entity</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Entity or null</returns>
        Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        /// <param name="autoSave">
        /// Set true to automatically save changes to database.
        /// This is useful for ORMs / database APIs those only save changes with an explicit method call, but you need to immediately save changes to the database.
        /// </param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default); //TODO: Return true if deleted

        /// <summary>
        /// Deletes multiple entities by primary keys.
        /// </summary>
        /// <param name="ids">Primary keys of the each entity.</param>
        /// <param name="autoSave">
        /// Set true to automatically save changes to database.
        /// This is useful for ORMs / database APIs those only save changes with an explicit method call, but you need to immediately save changes to the database.
        /// </param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Awaitable <see cref="Task" />.</returns>
        Task DeleteManyAsync([NotNull] IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据 Id 获取一个单一对象，如果没有或者有多个，抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Single(TKey id);

        /// <summary>
        /// 异步方法，根据 Id 获取一个单一对象，如果没有或者有多个，抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        Task<TEntity> SingleAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据 Id 获取一个单一对象，如果没有或者有多个，抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        TEntity SingleOrDefault(TKey id);

        /// <summary>
        /// 异步方法，根据 Id 获取一个单一对象，如果没有或者有多个，抛出异常
        /// </summary>
        /// <param name="predicate"></param>
        Task<TEntity> SingleOrDefaultAsync(TKey id, CancellationToken cancellationToken = default);
    }
}