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
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        Guid Id { get; }

        bool IsCompleted { get; }

        bool IsDisposed { get; }

        bool IsReserved { get; }

        bool IsSupportTransaction { get; }

        bool HasTransaction { get; }

        bool Activated { get; }

        string ReservationKey { get; }

        // IDictionary<object,object> Items { get; }

        IUnitOfWork Outer { get; set; }

        TransactionOptions Options { get; }

        void Active(TransactionOptions options = null);

        void SetOptions(TransactionOptions options);

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

        event EventHandler OnCompleted;
        event EventHandler OnFailed;
        event EventHandler OnDisposed;
    }
}