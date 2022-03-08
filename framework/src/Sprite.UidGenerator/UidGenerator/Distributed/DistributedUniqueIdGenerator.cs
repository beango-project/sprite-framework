using Microsoft.Extensions.Options;

namespace Sprite.UidGenerator
{
    public abstract class DistributedUniqueIdGeneratorBase<T> : IDistributedUniqueIdGenerator<T>
    {
        private readonly IUidProvider<T> _provider;

        protected DistributedUniqueIdGeneratorBase(IUidProvider<T> provider)
        {
            _provider = provider;
        }


        public virtual T NextId() => UiProvider.Create();


        public virtual IUidProvider<T> UiProvider => _provider;
    }

    public class DistributedUniqueIdGenerator : DistributedUniqueIdGeneratorBase<long>, IDistributedUniqueIdGenerator
    {
        public DistributedUniqueIdGenerator(IUidProvider<long> provider) : base(provider)
        {
        }
    }
}