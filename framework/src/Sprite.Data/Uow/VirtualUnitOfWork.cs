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
        public Guid Id => BaseUow.Id;
        public bool IsDisposed => BaseUow.IsDisposed;
        public bool IsReserved => BaseUow.IsReserved;
        public bool IsCompleted => BaseUow.IsCompleted;

        public bool IsSupportTransaction => BaseUow.IsSupportTransaction;
        public bool HasTransaction => BaseUow.HasTransaction;
        public bool Activated => BaseUow.Activated;
        public string ReservationKey => BaseUow.ReservationKey;

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


        public void Active(TransactionOptions options = null)
        {
            BaseUow.Active(options);
        }

        public void SetOptions(TransactionOptions options)
        {
            BaseUow.SetOptions(options);
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

        public void Dispose()
        {
            OnDisposed?.Invoke(this, null);
            Unsubscribe();
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }

        protected virtual void Unsubscribe()
        {
            foreach (var e in OnDisposed.GetInvocationList())
            {
                OnDisposed -= (EventHandler)e;
            }
        }
    }
}