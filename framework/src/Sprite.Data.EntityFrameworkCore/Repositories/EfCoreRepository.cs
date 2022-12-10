using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FastExpressionCompiler;
using Microsoft.EntityFrameworkCore;
using Sprite.Data.Entities;
using Sprite.Data.Exceptions;
using Sprite.Data.Repositories;
using Z.EntityFramework.Plus;

namespace Sprite.Data.EntityFrameworkCore.Repositories
{
    public class EfCoreRepository<TDbContext, TEntity> : RepositoryBase<TEntity>, IEfCoreRepository<TEntity>
        where TDbContext : DbContextBase<TDbContext>
        where TEntity : class, IEntity
    {
        private readonly IDbContextProvider<TDbContext> _dbContextProvider;


        public EfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
            // DbContext = _dbContextProvider.GetDbContext();
        }

        public DbContext DbContext => _dbContextProvider.GetDbContext();
        public DbSet<TEntity> DbSet => DbContext.Set<TEntity>();


        public virtual async Task<DbContext> GetDbContextAsync()
        {
            return await _dbContextProvider.GetDbContextAsync();
        }

        public override TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public override IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public override IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            IQueryable<TEntity> query = DbSet;
            if (propertySelectors != null)
            {
                foreach (var includeProperty in propertySelectors)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query;
        }

        public override List<TEntity> GetList()
        {
            return DbSet.ToList();
        }

        public override List<TEntity> GetList(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAll(propertySelectors).ToList();
        }

        public override async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync()).ToListAsync(cancellationToken);
        }

        public override async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            IQueryable<TEntity> query = null;

            if (propertySelectors != null)
            {
                foreach (var includeProperty in propertySelectors)
                {
                    if (query == null)
                    {
                        query = DbSet.Include(includeProperty);
                        continue;
                    }

                    query = query.Include(includeProperty);
                }
            }
            else
            {
                query = DbSet;
            }

            return await query.ToListAsync(cancellationToken);
        }

        public override IQueryable<TEntity> GetAllMerge(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = DbSet.Where(predicate);

            // var compileFast = predicate.CompileFast();
            DbSet.Where(predicate);
            if (propertySelectors != null)
            {
                foreach (var includeProperty in propertySelectors)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query;
        }

        public override async Task<IQueryable<TEntity>> GetAllMergeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return await base.GetAllMergeAsync(predicate, cancellationToken, propertySelectors);
        }


        public override async Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = DbSet.AsQueryable();

            if (propertySelectors != null)
            {
                foreach (var includeProperty in propertySelectors)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await Task.FromResult(query);
        }

        public override IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public override async Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(Find(predicate));
        }

        public override async Task<TEntity> AddAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entityEntry = (await DbContext.AddAsync(entity, cancellationToken)).Entity;

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }

            return entityEntry;
        }

        public override async Task AddManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            //TODO :修改改为追踪方式
            await DbContext.AddRangeAsync(entities, cancellationToken);
            // await DbSet.BulkInsertAsync(entities, cancellationToken);

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public override async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            DbContext.Attach(entity);
            var entityEntry = DbContext.Update(entity).Entity;

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }

            return entityEntry;
        }


        /// <summary>
        /// Update without loading
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="expression"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> expression, bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            //TODO :修改改为追踪方式
            return await DbSet.Where(predicate).UpdateAsync(expression, cancellationToken: cancellationToken);
        }


        public override async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            //TODO :修改改为追踪方式
            await DbContext.BulkUpdateAsync(entities, cancellationToken);

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public override async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            //TODO :修改改为追踪方式
            // await DbSet.SingleDeleteAsync(entity, cancellationToken);
            // DbSet.Attach(entity);
            DbContext.Set<TEntity>().Remove(entity);
            if (autoSave)
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public override async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var queryable = DbSet.Where(predicate);
            DbSet.RemoveRange(queryable);
            var res = queryable.DeferredCount().FutureValue();

            if (autoSave)
            {
                var saveCount = await DbContext.SaveChangesAsync(cancellationToken);
                return res == saveCount ? saveCount : 0;
            }

            return res;
        }

        public override async Task<int> DeleteManyAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            //TODO :修改改为追踪方式
            // var res = await DbSet.Where(predicate).DeleteAsync(cancellationToken);
            var queryable = DbSet.Where(predicate);
            DbSet.RemoveRange(queryable);
            var res = queryable.DeferredCount().FutureValue();

            if (autoSave)
            {
                var saveCount = await DbContext.SaveChangesAsync(cancellationToken);
                return res == saveCount ? saveCount : 0;
            }

            return res;
        }

        public override async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            //TODO :修改改为追踪方式
            // await DbSet.BulkDeleteAsync(entities, cancellationToken);
            DbSet.RemoveRange(entities);
            if (autoSave)
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public override TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Single(predicate);
        }

        public override async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.SingleAsync(predicate, cancellationToken);
        }

        public override TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return DbSet.SingleOrDefault(predicate);
        }

        public override async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        protected virtual async Task<DbSet<TEntity>> GetDbSetAsync()
        {
            return (await GetDbContextAsync()).Set<TEntity>();
        }
    }

    public class EfCoreRepository<TDbContext, TEntity, TKey> : EfCoreRepository<TDbContext, TEntity>, IEfCoreRepository<TEntity, TKey>
        where TDbContext : DbContextBase<TDbContext>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public EfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual TEntity Get(TKey id)
        {
            // var entity = DbSet.FirstOrDefault(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id));
            var entity = DbSet.FirstOrDefault(x => x.Id.Equals(id));
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public TEntity Get(TKey id, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = DbSet.AsQueryable();
            if (propertySelectors != null)
            {
                foreach (var includeProperty in propertySelectors)
                {
                    query = query.Include(includeProperty);
                }
            }

            var entity = query.FirstOrDefault(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id));
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FirstOrDefaultAsync(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id), cancellationToken);
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = DbSet.AsQueryable();
            if (propertySelectors != null)
            {
                foreach (var includeProperty in propertySelectors)
                {
                    query = query.Include(includeProperty);
                }
            }

            var entity = await query.FirstOrDefaultAsync(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id), cancellationToken: cancellationToken);
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public virtual TEntity Find(TKey id)
        {
            var entity = DbSet.Find(new { id });
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        public virtual async Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
                if (entity == null)
                {
                    throw new EntityNotFoundException(typeof(TEntity), id);
                }

                return entity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            // var entity = await DbSet.FirstOrDefaultAsync(x=>x.Id.Equals(id),cancellationToken);
            var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
            {
                return;
            }

            await DeleteAsync(entity, autoSave, cancellationToken);
        }

        public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            //TODO :修改改为追踪方式
            var res = await DbSet.Where(x => ids.Contains(x.Id)).DeleteAsync(cancellationToken);
            if (autoSave)
            {
                await SaveChangesAsync(cancellationToken);
            }
        }

        public TEntity Single(TKey id)
        {
            return DbSet.Single(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id));
        }

        public Task<TEntity> SingleAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return DbSet.SingleAsync(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id), cancellationToken: cancellationToken);
        }

        public TEntity SingleOrDefault(TKey id)
        {
            return DbSet.SingleOrDefault(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id));
        }

        public Task<TEntity> SingleOrDefaultAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return DbSet.SingleOrDefaultAsync(EntityHelper.BuildEntityEqualityExpressionFor<TEntity, TKey>(id), cancellationToken: cancellationToken);
        }
    }
}