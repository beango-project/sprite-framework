using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sprite.Data.Persistence;

namespace Sprite.Data.Ado
{
    public class Ado<TDbConnection> : IVendor, ISupportTransaction
        where TDbConnection : DbConnection
    {
        private readonly ConcurrentStack<DbTransaction> _txStack;
        private readonly ConcurrentStack<DbCommand> _commands;

        public Ado(TDbConnection dbConnection)
        {
            _txStack = new ConcurrentStack<DbTransaction>();
            _commands = new ConcurrentStack<DbCommand>();
            DbConnection = dbConnection;
        }

        // protected TDbConnection DbConnection { get; }


        public DbCommand CreateCommand()
        {
            var command = DbConnection.CreateCommand();
            _commands.Push(command);
            return command;
        }


        public DbTransaction? CurrentTransaction => GetCurrentTransaction();


        protected DbTransaction? GetCurrentTransaction()
        {
            _txStack.TryPeek(out var transaction);
            return transaction ?? null;
        }

        public DbTransaction BeginTransaction()
        {
            var transaction = DbConnection.BeginTransaction();
            _txStack.Push(transaction);
            return transaction;
        }

        public void UseTransaction(DbTransaction transaction)
        {
            _commands.TryPeek(out var dbCommand);
            dbCommand.Transaction = transaction;
        }

        public void Commit()
        {
            using (var enumerator = _commands.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Connection?.State != ConnectionState.Closed || enumerator.Current.Connection?.State != ConnectionState.Broken)
                    {
                        enumerator.Current.Transaction.Commit();
                        enumerator.Current.Transaction.Dispose();
                    }
                }
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            // await CurrentTransaction?.CommitAsync(cancellationToken);
        }

        public void Rollback()
        {
            CurrentTransaction?.Rollback();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await CurrentTransaction?.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            _txStack.Clear();
            _commands.Clear();
        }

        public DbConnection DbConnection { get; }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
    }
}