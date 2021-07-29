using System;
using System.Data;

namespace Sprite.Data.Transaction
{
    public interface ITransactionDescriptor
    {
        TransactionPropagation Propagation { get; }

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// 超时
        /// </summary>
        public TimeSpan? Timeout { get; set; }
    }
}