using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Entities;
using Sprite.Data.Exceptions;

namespace Sprite.Data.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        public abstract TEntity Get(Expression<Func<TEntity, bool>> predicate);

        public abstract IQueryable<TEntity> GetAll();

        public abstract IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] propertySelectors);

        public virtual List<TEntity> GetList()
        {
            return GetAll().ToList();
        }

        public virtual List<TEntity> GetList(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAll(propertySelectors).ToList();
        }

        public virtual async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            var query = await GetAllAsync(cancellationToken);
            return query.ToList();
        }

        public virtual async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = await GetAllAsync(cancellationToken, propertySelectors);
            return query.ToList();
        }

        public abstract IQueryable<TEntity> GetAllMerge(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);


        public virtual Task<IQueryable<TEntity>> GetAllMergeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity,
                object>>[] propertySelectors)
        {
            return Task.FromResult(GetAllMerge(predicate, propertySelectors));
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var query = await FindAsync(predicate, cancellationToken);
            return query.SingleOrDefault();
        }

        public virtual Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return Task.FromResult(GetAll(propertySelectors));
        }

        public abstract IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        public abstract Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        public abstract Task<TEntity> AddAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        public virtual async Task AddManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await AddAsync(entity, cancellationToken: cancellationToken);
            }

            if (autoSave)
            {
                await SaveChangesAsync(cancellationToken);
            }
        }

        public abstract Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await UpdateAsync(entity, cancellationToken: cancellationToken);
            }

            if (autoSave)
            {
                await SaveChangesAsync(cancellationToken);
            }
        }

        public abstract Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        public abstract Task<int> DeleteManyAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default);

        public abstract Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public virtual async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var query = await GetAllAsync(cancellationToken);
            return query.Single();
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleOrDefault(predicate);
        }

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var query = await GetAllAsync(cancellationToken);
            query.Where(predicate);
            return query.SingleOrDefault();
        }

        public virtual IEnumerator<TEntity> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType => GetAll().ElementType;
        public Expression Expression => GetAll().Expression;

        public IQueryProvider Provider => GetAll().Provider;
        // [Autowired] public IUnitOfWorkManager UnitOfWorkManager { get; }


        protected virtual int SaveChanges()
        {
            // if (UnitOfWorkManager?.CurrentUow != null)
            // {
            //     return UnitOfWorkManager.CurrentUow.SaveChanges();
            // }

            return 0;
        }

        protected virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // if (UnitOfWorkManager?.CurrentUow != null)
            // {
            //     return UnitOfWorkManager.CurrentUow.SaveChangesAsync(cancellationToken);
            // }

            return Task.FromResult(0);
        }
    }

    public abstract class RepositoryBase<TEntity, TKey> : RepositoryBase<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        public virtual TEntity Get(TKey id)
        {
            var entity = Find(id);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public virtual Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Get(id));
        }

        public abstract TEntity Find(TKey id);


        public virtual Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(id));
        }

        public abstract Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);


        public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            foreach (var id in ids)
            {
                await DeleteAsync(id, autoSave, cancellationToken);
            }
        }

        public virtual TEntity Single(TKey id)
        {
            return Find(id);
        }

        public virtual Task<TEntity> SingleAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return FindAsync(id, cancellationToken);
        }

        public virtual TEntity SingleOrDefault(TKey id)
        {
            return Find(id);
        }

        public virtual Task<TEntity> SingleOrDefaultAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return FindAsync(id, cancellationToken);
        }
    }
}