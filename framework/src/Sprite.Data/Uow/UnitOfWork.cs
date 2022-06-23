using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ImTools;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Nito.Disposables;
using Sprite.Data.Entities;
using Sprite.Data.Persistence;
using Sprite.Data.Repositories;
using Sprite.Data.Transaction;
using Sprite.DependencyInjection;

namespace Sprite.Data.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _isCompleting;
        private bool _isRolledback;

        public Guid Id { get; }
        public bool IsDisposed { get; private set; }

        public bool IsReserved { get; private set; }

        public bool IsCompleted { get; private set; }
        public bool IsSupportTransaction { get; private set; }

        public virtual bool HasTransaction => CheckTransactions();

        public bool Activated { get; private set; }

        public string ReservationKey { get; }

        public IUnitOfWork Outer { get; set; }

        public TransactionOptions Options { get; private set; }

        private ImHashMap<string, IVendor> _vendorMap;
        private ImHashMap<string, DbTransaction> _txMap;
        private Exception _exception;

        public UnitOfWork(TransactionOptions options)
        {
            Options = Check.NotNull(options, nameof(options));
            Id = Guid.NewGuid();
            _vendorMap = ImHashMap<string, IVendor>.Empty;
            _txMap = ImHashMap<string, DbTransaction>.Empty;
            Active();
        }

        public UnitOfWork(string reservationKey)
        {
            ReservationKey = Check.NotNullOrEmpty(reservationKey, nameof(reservationKey));
            Id = Guid.NewGuid();
            _vendorMap = ImHashMap<string, IVendor>.Empty;
            _txMap = ImHashMap<string, DbTransaction>.Empty;
            IsReserved = true;
        }

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
                OnFailed?.Invoke(this, null);
            }

            OnDisposed?.Invoke(this, null);
            _vendorMap = null;
            _txMap = null;

            Unsubscribe();
        }


        public void Active(TransactionOptions options = null)
        {
            if (Activated)
            {
                throw new Exception("This unit of work has been activated before!");
            }

            Options = options ?? Options;
            Activated = true;
            IsReserved = false;
            SetTransactionPropagation();
        }

        public void SetOptions(TransactionOptions options)
        {
            Options = Check.NotNull(options, nameof(options));
        }

        public IVendor GetOrAddVendor(string key, IVendor vendor)
        {
            // _vendorMap[vendor.DbConnection].Add(vendor);
            if (_vendorMap.TryFind(key, out _))
            {
                return vendor;
            }

            if (!Activated)
            {
                Active();
            }

            _vendorMap = _vendorMap.AddOrUpdate(key, vendor);
            return vendor;
        }


        public IVendor FindVendor(string key)
        {
            return _vendorMap.GetValueOrDefault(key);
        }

        public DbTransaction FindDbTransaction(string key)
        {
            return _txMap.GetValueOrDefault(key);
        }

        public void AddDbTransaction(string key, DbTransaction dbTransaction)
        {
            if (!IsSupportTransaction) //非事务性的不能添加事务
            {
                return;
            }

            Check.NotNull(key, nameof(key));
            Check.NotNull(dbTransaction, nameof(dbTransaction));

            if (_txMap.Contains(key))
            {
                throw new Exception("There is already a transaction API in this unit of work with given key: " + key);
            }

            if (!Activated)
            {
                Active();
            }

            _txMap = _txMap.AddOrUpdate(key, dbTransaction);
        }

        public IReadOnlyList<IVendor> GetVendors()
        {
            if (!_vendorMap.IsEmpty)
            {
                return _vendorMap.Enumerate().Select(map => map.Value).ToImmutableList();
            }

            return null;
        }


        public int SaveChanges()
        {
            if (!_vendorMap.IsEmpty)
            {
                int saveCount = 0;

                foreach (var entry in _vendorMap.Enumerate())
                {
                    if (entry.Value is ISupportPersistent persistent)
                    {
                        saveCount += persistent.SaveChanges();
                    }
                }

                return saveCount;
            }

            return 0;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (!_vendorMap.IsEmpty)
            {
                int saveCount = 0;


                foreach (var entry in _vendorMap.Enumerate())
                {
                    if (entry.Value is ISupportPersistent persistent)
                    {
                        saveCount += await persistent.SaveChangesAsync(cancellationToken);
                    }
                }

                return saveCount;
            }

            return await Task.FromResult<int>(0);
        }

        protected virtual void Commit()
        {
            foreach (var entry in _vendorMap.Enumerate())
            {
                if (entry.Value is ISupportTransaction transaction)
                {
                    transaction.Commit();
                }
            }
        }

        protected virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in _vendorMap.Enumerate())
            {
                if (entry.Value is ISupportTransaction transaction)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }

            // foreach (var entry in _txMap.Enumerate())
            // {
            //     entry.Value.Commit();
            // }
        }

        public void Rollback()
        {
            if (_isRolledback)
            {
                return;
            }

            _isRolledback = true;

            foreach (var entry in _vendorMap.Enumerate())
            {
                if (entry.Value is ISupportTransaction transaction)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }
                }
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_isRolledback)
            {
                return;
            }

            _isRolledback = true;

            foreach (var entry in _vendorMap.Enumerate())
            {
                try
                {
                    if (entry.Value is ISupportTransaction transaction)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                    }
                }
                catch
                {
                }
            }

            foreach (var txEntry in _txMap.Enumerate())
            {
                try
                {
                    if (txEntry.Value.Connection != null)
                    {
                        await txEntry.Value.RollbackAsync(cancellationToken);
                    }
                }
                catch
                {
                }
            }
        }

        public virtual void Completed()
        {
            if (_isRolledback)
            {
                return;
            }

            PreventMultipleCompletions();

            try
            {
                _isCompleting = true;
                SaveChanges();
                Commit();
                IsCompleted = true;
                OnCompleted?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                _exception = ex;
                Console.WriteLine(_exception);
                throw;
            }
        }

        public virtual async Task CompletedAsync(CancellationToken cancellationToken = default)
        {
            if (_isRolledback)
            {
                return;
            }

            PreventMultipleCompletions();

            try
            {
                _isCompleting = true;
                await SaveChangesAsync(cancellationToken);
                await CommitAsync(cancellationToken);
                IsCompleted = true;
                OnCompleted?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        public event EventHandler OnCompleted;
        public event EventHandler OnFailed;
        public event EventHandler OnDisposed;


        private void PreventMultipleCompletions()
        {
            if (IsCompleted || _isCompleting)
            {
                throw new Exception("Complete is called before!");
            }
        }


        protected bool CheckTransactions()
        {
            return _vendorMap.Enumerate().Any(x =>
            {
                if (x.Value is ISupportTransaction supportTransaction && supportTransaction.CurrentTransaction != null)
                {
                    return true;
                }

                return false;
            }) && _txMap.IsEmpty;
        }

        protected virtual void SetTransactionPropagation()
        {
            switch (Options.Propagation)
            {
                case Propagation.Required:
                case Propagation.Auto:
                case Propagation.Supports:
                case Propagation.RequiresNew:
                case Propagation.Mandatory:
                case Propagation.Nested:
                    IsSupportTransaction = true;
                    break;
                case Propagation.NotSupported:
                case Propagation.Never:
                    IsSupportTransaction = false;
                    break;
            }
        }

        private void DisposeTransactions()
        {
            // foreach (var entry in _vendorMap.Enumerate())
            // {
            //     try
            //     {
            //         entry.Value.Dispose();
            //     }
            //     catch
            //     {
            //     }
            // }

            foreach (var entry in _txMap.Enumerate())
            {
                try
                {
                    entry.Value.Dispose();
                }
                catch
                {
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            // foreach (var entry in _vendorMap.Enumerate())
            // {
            //     try
            //     {
            //         await entry.Value.DisposeAsync();
            //     }
            //     catch
            //     {
            //     }
            // }

            foreach (var entry in _txMap.Enumerate())
            {
                try
                {
                    await entry.Value.DisposeAsync();
                }
                catch
                {
                }
            }

            if (!IsCompleted || _exception != null)
            {
                OnFailed?.Invoke(this, null);
            }

            OnDisposed?.Invoke(this, null);
        }

        /// <summary>
        /// 清除事件处理器订阅列表，防止内存泄漏
        /// </summary>
        protected virtual void Unsubscribe()
        {
            if (OnCompleted != null)
            {
                foreach (var e in OnCompleted.GetInvocationList())
                {
                    OnCompleted -= (EventHandler)e;
                }
            }

            if (OnFailed != null)
            {
                foreach (var e in OnFailed.GetInvocationList())
                {
                    OnFailed -= (EventHandler)e;
                }
            }

            if (OnDisposed != null)
            {
                foreach (var e in OnDisposed.GetInvocationList())
                {
                    OnDisposed -= (EventHandler)e;
                }
            }
        }
    }
}