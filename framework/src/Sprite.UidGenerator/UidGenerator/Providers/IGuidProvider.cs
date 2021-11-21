using System;
using Sprite.UidGenerator;

namespace Sprite.UidGenerator.Providers
{
    public interface IGuidProvider : IUidProvider<Guid>
    {
    }
}