// using System;
// using System.Collections.Generic;
// using System.Data.Common;
// using System.Threading;
// using System.Threading.Tasks;
// using ImTools

// using Sprite.Data.Persistence;
// using Sprite.Data.Transaction;
//
// namespace Sprite.Data.Uow
// {
//     public class NonTransactionalUnitOfWork : IUnitOfWork
//     {
//         public NonTransactionalUnitOfWork()
//         {
//         }
//
//         public NonTransactionalUnitOfWork(IUnitOfWork baseUow)
//         {
//             BaseUow = baseUow;
//         }
//
//         public void Dispose()
//         {
//             OnDisposed?.Invoke(this, null);
//         }
//
//         public Guid Id => BaseUow.Id;
//         public bool IsDisposed { get; }
//         public bool IsCompleted { get; }
//
//         public IUnitOfWork Outer
//         {
//             get => BaseUow.Outer;
//             set => BaseUow.Outer = value;
//         }
//
//         public TransactionOptions Options { get; }
//
//         public IUnitOfWork BaseUow { get; }
//
//         private ImHashMap<string, IVendor> _vendorMap;
//         private ImHashMap<string, DbTransaction> _txMap;
//
//         public IVendor GetOrAddVendor(string key, IVendor vendor)
//         {
//
//             if (_vendorMap.TryFind(key, out _))
//             {
//                 return vendor;
//             }
//
//             _vendorMap = _vendorMap.AddOrUpdate(key, vendor);
//             return vendor;
//         }
//
//         public IVendor FindVendor(string key)
//         {
//             return _vendorMap.GetValueOrDefault(key);
//         }
//
//         public DbTransaction FindDbTransaction(string key)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void AddDbTransaction(string key, DbTransaction dbTransaction)
//         {
//             throw new NotImplementedException();
//         }
//
//         public IReadOnlyList<IVendor> GetVendors()
//         {
//             throw new NotImplementedException();
//         }
//
//         public int SaveChanges()
//         {
//             throw new NotImplementedException();
//         }
//
//         public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Rollback()
//         {
//             throw new NotImplementedException();
//         }
//
//         public Task RollbackAsync(CancellationToken cancellationToken = default)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Completed()
//         {
//             throw new NotImplementedException();
//         }
//
//         public Task CompletedAsync(CancellationToken cancellationToken = default)
//         {
//             throw new NotImplementedException();
//         }
//
//         public event EventHandler OnCompleted;
//         public event EventHandler OnFailed;
//         public event EventHandler OnDisposed;
//     }
// }