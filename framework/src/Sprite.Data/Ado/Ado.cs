using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sprite.Data.Persistence;

namespace Sprite.Data.Ado
{
    public class Ado<TDbConnection> : IVendor, ISupportPersistent, ISupportTransaction
        where TDbConnection : DbConnection
    {
        [CanBeNull]
        private readonly DbTransaction _transaction;

        public Ado(TDbConnection dbConnection)
        {
            DbConnection = dbConnection;
            DbCommand = DbConnection.CreateCommand();
            _transaction = DbCommand.Transaction;
        }

        // protected TDbConnection DbConnection { get; }
        public DbCommand DbCommand { get; }

        public int SaveChanges()
        {
            var res = DbCommand.ExecuteNonQuery();
            // _transaction?.Commit();
            return res;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public DbTransaction? CurrentTransaction { get; }

        public DbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void UseTransaction(DbTransaction transaction)
        {
            DbCommand.Transaction = transaction;
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public DbConnection DbConnection { get; }
    }
}