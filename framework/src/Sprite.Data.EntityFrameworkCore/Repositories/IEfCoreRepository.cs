using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sprite.Data.Entities;
using Sprite.Data.Repositories;

namespace Sprite.Data.EntityFrameworkCore.Repositories
{
    public interface IEfCoreRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        DbContext DbContext { get; }

        DbSet<TEntity> DbSet { get; }

        Task<DbContext> GetDbContextAsync();
    }

    public interface IEfCoreRepository<TEntity, TKey> : IEfCoreRepository<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
    }
}