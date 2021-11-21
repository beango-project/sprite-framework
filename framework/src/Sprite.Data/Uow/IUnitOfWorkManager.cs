using System;
using JetBrains.Annotations;
using Sprite.Data.Transaction;

namespace Sprite.Data.Uow
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork? CurrentUow { get; }

        IUnitOfWork Begin(TransactionOptions options = null);
    }
}