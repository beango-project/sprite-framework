namespace Sprite.Transactions.Sagas
{
    public interface ISaga<TSagaData>
    {
        /// <summary>
        /// Global Transaction Id 
        /// </summary>
        string Id { get; }
        
        
        TSagaData SagaData { get; }
    }
}