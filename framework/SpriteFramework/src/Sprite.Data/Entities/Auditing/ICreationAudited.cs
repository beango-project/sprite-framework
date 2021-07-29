namespace Sprite.Data.Entities.Auditing
{
    public interface ICreationAudited<TCreator, TKey> : IHasCreator<TCreator, TKey>, IHasCreationTime
        where TCreator : IEntity<TKey>
    {
    }
}