using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Entities;
using Sprite.Data.Exceptions;
using Sprite.Data.Uow;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Data.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        [Autowired]
        public IUnitOfWorkManager UnitOfWorkManager { get; }

        [Autowired]
        protected virtual ICurrentPrincipalAccessor PrincipalAccessor { get; set; }

        protected virtual int SaveChanges()
        {
            if (UnitOfWorkManager?.CurrentUow != null)
            {
                return UnitOfWorkManager.CurrentUow.SaveChanges();
            }

            return 0;
        }

        protected virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (UnitOfWorkManager?.CurrentUow != null)
            {
                return UnitOfWorkManager.CurrentUow.SaveChangesAsync(cancellationToken);
            }

            return Task.FromResult(0);
        }

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

        public abstract Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> expression, bool autoSave = false,
            CancellationToken cancellationToken = default);


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

        public abstract Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default);


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

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
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
        //
        // /// <summary>
        // /// Get data filter with data permission.
        // /// Use FastExpressionCompiler.LightExpression<seealso cref="FastExpressionCompiler.LightExpression.Expression"/> build expression to get better performance
        // /// </summary>
        // /// <param name="operation"></param>
        // /// <returns></returns>
        // protected virtual FastExpressionCompiler.LightExpression.Expression<Func<TEntity, bool>> GetDataFilter(DataOperation operation)
        // {
        //     return GetDataFilterExpression<TEntity>(operation: operation).ToLambdaExpression();
        // }
        //
        // public virtual FastExpressionCompiler.LightExpression.Expression<Func<TEntity, bool>> GetDataFilterExpression<TEntity>(FilterGroup group = null,
        //     DataOperation operation = DataOperation.Read)
        // {
        //     //默认获取全部的 where(m=>true)
        //     var body = FastExpressionCompiler.LightExpression.Expression.Constant(true, typeof(TEntity));
        //     var para = FastExpressionCompiler.LightExpression.Expression.Parameter(typeof(TEntity), "m");
        //     var exp = FastExpressionCompiler.LightExpression.Expression.Lambda<Func<TEntity, bool>>(body, para);
        //     if (group != null)
        //     {
        //         exp = FilterHelper.GetExpression<TEntity>(group);
        //     }
        //
        //     //从缓存中查找当前用户的角色与实体T的过滤条件
        //     ClaimsPrincipal user = PrincipalAccessor.CurrentPrincipal;
        //     if (user == null)
        //     {
        //         return exp;
        //     }
        //
        //     IDataAuthCache dataAuthCache = ServiceLocator.Instance.GetService<IDataAuthCache>();
        //     if (dataAuthCache == null)
        //     {
        //         return exp;
        //     }
        //
        //     string[] roleNames = null;
        //     // 要判断数据权限功能,先要排除没有执行当前功能权限的角色,判断剩余角色的数据权限
        //     if (user.Identity is ClaimsIdentity claimsIdentity)
        //     {
        //         roleNames = claimsIdentity.FindAll(ClaimTypes.Role).SelectMany(m =>
        //         {
        //             var roles = m.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //             return roles;
        //         }).ToArray();
        //     }
        //     else
        //     {
        //         roleNames = Array.Empty<string>();
        //     }
        //
        //
        //     ScopedDictionary scopedDict = ServiceLocator.Instance.GetService<ScopedDictionary>();
        //     if (scopedDict?.Function != null)
        //     {
        //         roleNames = scopedDict.DataAuthValidRoleNames;
        //     }
        //
        //     string typeName = typeof(TEntity).FullName; //NOTE: Maybe it can become Module name + type name ?
        //     FastExpressionCompiler.LightExpression.Expression<Func<TEntity, bool>> subExp = null;
        //     foreach (string roleName in roleNames)
        //     {
        //         FilterGroup subGroup = dataAuthCache.GetFilterGroup(roleName, typeName, operation);
        //         if (subGroup == null)
        //         {
        //             continue;
        //         }
        //
        //         // 各个角色的数据过滤条件使用Or连接
        //         subExp = subExp == null ? FilterHelper.GetExpression<TEntity>(subGroup) : subExp.Or(FilterHelper.GetExpression<TEntity>(subGroup));
        //     }
        //
        //     if (subExp != null)
        //     {
        //         if (group == null)
        //         {
        //             return subExp;
        //         }
        //
        //         // 数据权限条件与主条件使用And连接
        //         exp = subExp.And(exp);
        //     }
        //
        //     return exp;
        // }
    }

    public abstract class RepositoryBase<TEntity, TKey> : RepositoryBase<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
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

        public abstract TEntity Get(TKey id, params Expression<Func<TEntity, object>>[] propertySelectors);


        public virtual Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Get(id));
        }

        public abstract Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] propertySelectors);


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