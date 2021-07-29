﻿using System;
using System.Data;
using Sprite.Data.Transaction;

namespace Sprite.Data.Uow
{
    public class UnitOfWorkOptions
    {
        public static UnitOfWorkOptions Default = new(TransactionPropagation.Required);

        public UnitOfWorkOptions()
        {
        }

        public UnitOfWorkOptions(TransactionPropagation propagation, IsolationLevel? isolationLevel = null, TimeSpan? timeout = null)
        {
            Propagation = propagation;
            IsolationLevel = isolationLevel;
            Timeout = timeout;
        }

        public TransactionPropagation Propagation { get; set; } = TransactionPropagation.Required;

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