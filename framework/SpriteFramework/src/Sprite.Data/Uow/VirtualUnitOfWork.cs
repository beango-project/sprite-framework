using System;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Persistence;
using Sprite.Data.Transaction;

namespace Sprite.Data.Uow
{
    public class VirtualUnitOfWork : IUnitOfWork
    {
        private readonly IUnitOfWork _baseUow;

        public VirtualUnitOfWork(IUnitOfWork baseUow)
        {
            _baseUow = baseUow;
        }

        public bool IsDisposed { get; }
        public bool IsCompleted { get; }
        public Guid Id { get; }
        public IVendor Vendor => _baseUow.Vendor;
        public TransactionOptions Options { get; }
        public event EventHandler Disposed;

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public int SaveChanges()
        {
            return _baseUow.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _baseUow.SaveChangesAsync(cancellationToken);
        }

        public void Commit()
        {
            //do nothing
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            //do nothing
            return Task.CompletedTask;
        }

        public void Rollback()
        {
            _baseUow.Rollback();
        }

        public Task RollBackAsync(CancellationToken cancellationToken)
        {
            return _baseUow.RollBackAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}