namespace Sprite.UidGenerator
{
    public class DistributedUniqueIdGenerator : IDistributedUniqueIdGenerator
    {
        private readonly ISequentialUidProvider<long> _uidProvider;

        public DistributedUniqueIdGenerator(ISequentialUidProvider<long> uidProvider)
        {
            _uidProvider = uidProvider;
        }

        public long NextId()
        {
            return _uidProvider.NextId();
        }
    }
}