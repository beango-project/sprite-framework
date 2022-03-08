using System;

namespace Sprite.UidGenerator
{
    public interface IDistributedUniqueIdGenerator<T>
    {
        T NextId();

        IUidProvider<T> UiProvider { get; }
    }

    public interface IDistributedUniqueIdGenerator : IDistributedUniqueIdGenerator<long>
    {
    }
}