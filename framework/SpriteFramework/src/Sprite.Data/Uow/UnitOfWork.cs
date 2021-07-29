using System;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Persistence;
using Sprite.Data.Transaction;
using Sprite.DependencyInjection;

namespace Sprite.Data.Uow
{
    public class UnitOfWork : IUnitOfWork, IScopeInjection
    {
        private Exception _exception;
        private bool _isCompleting;
        private bool _isRolledback;
        private bool _isTransaction = true;
        private IVendor _vendor;


        public UnitOfWork()
        {
            Id = Guid.NewGuid();
        }

        public UnitOfWork(IVendor vendor, TransactionOptions options)
        {
            _vendor = vendor;
            Options = options;
            Id = Guid.NewGuid();
        }

        public IServiceProvider ServiceProvider { get; }

        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            DisposeTransactions();

            if (!IsCompleted || _exception != null)
            {
                //   OnFailed();
            }

            // OnDisposed();
        }

        public Guid Id { get; }

        public Task RollBackAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool IsDisposed { get; private set; }
        public bool IsCompleted { get; private set; }
        public IVendor Vendor => _vendor;
        public event EventHandler Disposed;
        public TransactionOptions Options { get; private set; }

        // public IRepository<TEntity> GenericRepository<TEntity>() where TEntity : class, IEntity
        // {
        //     return ServiceProvider.GetRequiredService<IRepository<TEntity>>();
        // }
        //
        // public IRepository<TEntity, TKey> GenericRepository<TEntity, TKey>() where TEntity : class, IEntity<TKey>
        // {
        //     return ServiceProvider.GetRequiredService<IRepository<TEntity, TKey>>();
        // }

        public int SaveChanges()
        {
            if (_vendor is ISupportPersistent persistent)
            {
                return persistent.SaveChanges();
            }

            return 0;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_vendor is ISupportPersistent persistent)
            {
                return await persistent.SaveChangesAsync(cancellationToken);
            }

            return await Task.FromResult(0);
        }

        public void Commit()
        {
            if (_isTransaction)
            {
                return;
            }

            if (_vendor is ISupportTransaction transaction)
            {
                transaction.Commit();
            }

            _isCompleting = true;
            PreventMultipleSubmissions();
            IsCompleted = true;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_isTransaction)
            {
                return;
            }

            if (_vendor is ISupportTransaction transaction)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            _isCompleting = true;
            PreventMultipleSubmissions();
            IsCompleted = true;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Completed;
        public event EventHandler Failed;

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SetPersistenceVender(IVendor vendor)
        {
            // Vendor = Check.NotNull(vendor, nameof(vendor));
        }

        public void SetLinked(IUnitOfWork unitOfWork)
        {
            // Linked = unitOfWork;
        }


        // private void OnDisposed()
        // {
        //     Disposed.Invoke(this, EventArgs.Empty);
        // }
        //
        // private void OnFailed()
        // {
        //     throw new NotImplementedException();
        // }

        private void DisposeTransactions()
        {
            if (Vendor is ISupportTransaction)
            {
                try
                {
                    Vendor.Dispose();
                }
                catch
                {
                }
            }
        }

        private void PreventMultipleSubmissions()
        {
            if (IsCompleted || _isCompleting)
            {
                throw new Exception("Commit is called before!");
            }
        }
    }
}