﻿using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Sprite.Data.Persistence
{
    /// <summary>
    /// 支持事务
    /// </summary>
    public interface ISupportTransaction
    {
        DbTransaction? CurrentTransaction { get; }

        DbTransaction  BeginTransaction();
        
        void UseTransaction(DbTransaction transaction);
        
        void Commit();

        Task CommitAsync(CancellationToken cancellationToken);

        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken);
    }
}