using System;
using System.Data;
using Sprite.Data.Transaction;

namespace Sprite.Data.Uow
{
    public class UnitOfWorkOptions
    {

        public UnitOfWorkOptions()
        {
        }

        public UnitOfWorkOptions(Propagation propagation, IsolationLevel? isolationLevel = null, TimeSpan? timeout = null)
        {
            Propagation = propagation;
            IsolationLevel = isolationLevel;
            Timeout = timeout;
        }

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