using System.Threading;

namespace Sprite.Data.Transaction
{
    public class AmbientTransaction
    {
        public AmbientTransaction()
        {
            CurrentTransaction = new AsyncLocal<ITransactionInfo>();
        }

        public AsyncLocal<ITransactionInfo> CurrentTransaction { get; }
    }
}