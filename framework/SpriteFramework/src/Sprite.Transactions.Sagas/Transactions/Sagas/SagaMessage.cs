namespace Sprite.Transactions.Sagas
{
    public abstract class SagaMessage
    {
        public string TransactionId { get; }
    }
}