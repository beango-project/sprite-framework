using System;
using System.Data.Common;

namespace Sprite.Data.Transaction
{
    public interface ITransactionInfo : ISavepointManager, IDisposable
    {
        bool IsNewTransaction { get; }

        DbTransaction Transaction { get; }

        bool Completed { get; }

        bool fail { get; }
    }
}