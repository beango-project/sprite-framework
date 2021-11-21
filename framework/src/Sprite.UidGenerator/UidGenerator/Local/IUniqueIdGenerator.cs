using System;
using Sprite.UidGenerator;

namespace Sprite.UidGenerator
{
    public interface IUniqueIdGenerator
    {
        Guid Create();
    }
}