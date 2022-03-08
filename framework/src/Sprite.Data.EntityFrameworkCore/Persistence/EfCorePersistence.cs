using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Sprite.Data.Persistence;
using Sprite.Data.Uow;

namespace Sprite.Data.EntityFrameworkCore.Persistence
{
    public sealed class EfCorePersistence<TDbContext> : IEfCorePersistence<TDbContext>
        where TDbContext : DbContext
    {
        private bool _isDispose;


        public EfCorePersistence(TDbContext dbContext)
        {
            DbContext = dbContext;
            // HookUpDbContexts = new List<TDbContext>();
        }


        // public List<TDbContext> HookUpDbContexts { get; }
        public TDbContext DbContext { get; private set; }

        public IDbContextTransaction? DbContextTransaction => DbContext.Database.CurrentTransaction;

        public DbTransaction? CurrentTransaction => DbContextTransaction?.GetDbTransaction();

        public void Dispose()
        {
            if (_isDispose)
            {
                return;
            }

            _isDispose = true;

            DbContextTransaction?.Dispose();
            // DbContext.Dispose();
            // GC.SuppressFinalize(this);
        }


        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }


        public DbTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction().GetDbTransaction();
        }

        public void UseTransaction(DbTransaction transaction)
        {
            DbContext.Database.UseTransaction(transaction);
        }

        public void Commit()
        {
            // foreach (var dbContext in HookUpDbContexts)
            // {
            //     var currentTransaction = dbContext.Database.GetService<IDbContextTransactionManager>();
            //     if (currentTransaction != null && currentTransaction is IRelationalTransactionManager &&
            //         dbContext.Database.GetDbConnection() == DbContextTransaction.GetDbTransaction().Connection)
            //     {
            //         dbContext.Database.CommitTransaction();
            //     }
            // }

            DbContextTransaction?.Commit();
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            // foreach (var dbContext in HookUpDbContexts)
            // {
            //     var currentTransaction = dbContext.Database.GetService<IDbContextTransactionManager>();
            //     if (currentTransaction != null && currentTransaction is IRelationalTransactionManager &&
            //         dbContext.Database.GetDbConnection() == DbContextTransaction.GetDbTransaction().Connection)
            //     {
            //         await dbContext.Database.CommitTransactionAsync(cancellationToken);
            //     }
            // }

            if (DbContextTransaction != null)
            {
                await DbContextTransaction.CommitAsync(cancellationToken);
            }
        }

        public void Rollback()
        {
            DbContextTransaction?.Rollback();
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (DbContextTransaction != null)
            {
                return DbContextTransaction.RollbackAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        // public void SetTransaction(IDbContextTransaction dbContextTransaction)
        // {
        //     DbContextTransaction = dbContextTransaction;
        // }

        public bool IsDispose => _isDispose;

        public DbConnection DbConnection => DbContext.Database.GetDbConnection();

        public ValueTask DisposeAsync()
        {
            if (_isDispose)
            {
                return ValueTask.CompletedTask;
            }

            _isDispose = true;
            return ValueTask.CompletedTask;
        }
    }
}