using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Persistence;
using Sprite.Data.Transaction;

namespace Sprite.Data.Uow
{
    public class VirtualUnitOfWork : IUnitOfWork
    {
        public void Dispose()
        {
            OnDisposed?.Invoke(this, null);
        }

        public Guid Id => BaseUow.Id;
        public bool IsDisposed => BaseUow.IsDisposed;
        public bool IsCompleted => BaseUow.IsCompleted;

        public bool IsSupportTransaction => BaseUow.IsSupportTransaction;

        public IUnitOfWork Outer
        {
            get => BaseUow.Outer;
            set => BaseUow.Outer = value;
        }

        public TransactionOptions Options { get; }

        public IUnitOfWork BaseUow { get; }

        public VirtualUnitOfWork(TransactionOptions options, IUnitOfWork baseUow)
        {
            Options = options;
            BaseUow = baseUow;
            BaseUow.OnFailed += (sender, args) => { OnFailed?.Invoke(sender, args); };
            BaseUow.OnCompleted += (sender, args) => { OnCompleted?.Invoke(sender, args); };
            // BaseUow.OnDisposed += (sender, args) => { OnDisposed?.Invoke(sender, args); };
        }

        public IVendor GetOrAddVendor(string key, IVendor vendor)
        {
            return BaseUow.GetOrAddVendor(key, vendor);
        }

        public IVendor FindVendor(string key)
        {
            return BaseUow.FindVendor(key);
        }

        public DbTransaction FindDbTransaction(string key)
        {
            return BaseUow.FindDbTransaction(key);
        }

        public void AddDbTransaction(string key, DbTransaction dbTransaction)
        {
            BaseUow.AddDbTransaction(key, dbTransaction);
        }

        public IReadOnlyList<IVendor> GetVendors()
        {
            return BaseUow.GetVendors();
        }

        public int SaveChanges()
        {
            return BaseUow.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return BaseUow.SaveChangesAsync(cancellationToken);
        }


        public void Rollback()
        {
            BaseUow.Rollback();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return BaseUow.RollbackAsync(cancellationToken);
        }

        public void Completed()
        {
        }

        public Task CompletedAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public event EventHandler OnCompleted;
        public event EventHandler OnFailed;
        public event EventHandler OnDisposed;
    }
}