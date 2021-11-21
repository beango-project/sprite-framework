namespace Sprite.Transactions.Sagas
{
    public enum SagaState
    {
        Pending,
        Succeed,
        Cancelled,
        Fail
    }
}