using System;
using System.Data;

namespace Sprite.Data.Transaction
{
    public class TransactionOptions
    {
        public Propagation Propagation { get; set; } = Propagation.Required;

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