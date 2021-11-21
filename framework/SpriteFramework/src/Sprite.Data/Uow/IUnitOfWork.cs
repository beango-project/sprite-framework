using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sprite.Data.Entities;
using Sprite.Data.Persistence;
using Sprite.Data.Repositories;
using Sprite.Data.Transaction;
using Sprite.DependencyInjection;

namespace Sprite.Data.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }

        bool IsDisposed { get; }

        bool IsCompleted { get; }

        bool IsSupportTransaction { get; }
        
        IUnitOfWork Outer { get; set; }

        TransactionOptions Options { get; }

        IVendor GetOrAddVendor(string key, IVendor vendor);

        [CanBeNull]
        IVendor FindVendor(string key);

        [CanBeNull]
        DbTransaction FindDbTransaction(string key);

        void AddDbTransaction(string key, DbTransaction dbTransaction);


        IReadOnlyList<IVendor> GetVendors();

        int SaveChanges();


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


        void Rollback();


        Task RollbackAsync(CancellationToken cancellationToken = default);

        void Completed();

        Task CompletedAsync(CancellationToken cancellationToken = default);

        public event EventHandler OnCompleted;
        public event EventHandler OnFailed;
        public event EventHandler OnDisposed;
    }
}