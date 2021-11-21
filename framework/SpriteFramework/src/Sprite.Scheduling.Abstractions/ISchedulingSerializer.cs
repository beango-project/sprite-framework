using System;

namespace Sprite.Scheduling.Abstractions
{
    public interface ISchedulingSerializer
    {
        string Serialize(object obj);

        object Deserialize(string value, Type type);
    }
}