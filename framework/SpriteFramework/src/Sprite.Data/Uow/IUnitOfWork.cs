using System;
using System.Threading;
using System.Threading.Tasks;
using Sprite.Data.Persistence;
using Sprite.Data.Transaction;

namespace Sprite.Data.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }

        IVendor Vendor { get; }

        TransactionOptions Options { get; }

        /// <summary>
        /// 是否被销毁
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsCompleted { get; }

        event EventHandler Disposed;

        public int SaveChanges();


        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 异步提交事务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 回滚
        /// </summary>
        void Rollback();

        /// <summary>
        /// 异步回滚
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RollBackAsync(CancellationToken cancellationToken);
    }
}