namespace Sprite.Data.Entities.Auditing
{
    public interface IHasCreator<TCreator, TKey>
        where TCreator : IEntity<TKey>
    {
        [CreateBy]
        TCreator CreateUser { get; }
    }
}