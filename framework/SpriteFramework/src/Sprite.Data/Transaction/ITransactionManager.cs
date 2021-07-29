namespace Sprite.Data.Transaction
{
    public interface ITransactionManager
    {
        ITransactionInfo CurrentTransaction { get; }

        ITransactionInfo BeginTransaction(ITransactionDescriptor descriptor);
    }
}