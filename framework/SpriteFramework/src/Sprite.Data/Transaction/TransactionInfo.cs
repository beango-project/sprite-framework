using System;
using System.Data.Common;
using Sprite.Data.Persistence;

namespace Sprite.Data.Transaction
{
    public class TransactionInfo : ITransactionInfo
    {
        private readonly IVendor _vendor;

        private string _savepointName;

        public TransactionInfo(DbTransaction transaction, IVendor vendor, bool isNewTransaction)
        {
            Transaction = transaction;
            _vendor = vendor;
            IsNewTransaction = isNewTransaction;
        }

        private bool _isCompleted { get; }


        public bool SupportSavepoint => _vendor is ISupportSavepoint;
        public bool IsNewTransaction { get; }

        public virtual DbTransaction Transaction { get; }

        public bool Completed => _isCompleted;

        public bool fail { get; }


        public virtual void CreateSavepoint(string savepointName)
        {
            if (SupportSavepoint)
            {
                _savepointName = savepointName;
                Transaction.Save(savepointName);
                (_vendor as ISupportSavepoint)?.Save(savepointName);
            }
        }

        public virtual void RollbackToSavepoint(string savepoint)
        {
            if (SupportSavepoint)
            {
                (_vendor as ISupportSavepoint)?.Rollback(savepoint);
            }
        }

        public virtual void ReleaseSavepoint(string savepoint)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            Transaction?.Dispose();
        }
    }
}