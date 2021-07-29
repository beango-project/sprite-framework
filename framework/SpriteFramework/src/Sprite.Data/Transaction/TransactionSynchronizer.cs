using System.Collections.Generic;
using Sprite.Data.Persistence;

namespace Sprite.Data.Transaction
{
    public class TransactionSynchronizer
    {
        private Dictionary<IVendor, TransactionInfo> _transactions;

        public TransactionSynchronizer()
        {
            _transactions = new Dictionary<IVendor, TransactionInfo>();
        }
    }
}