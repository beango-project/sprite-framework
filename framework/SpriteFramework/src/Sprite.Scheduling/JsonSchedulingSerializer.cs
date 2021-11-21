using System;
using System.Text.Json;
using Sprite.Scheduling.Abstractions;

namespace Sprite.Scheduling
{
    public class JsonSchedulingSerializer : ISchedulingSerializer
    {
        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public object Deserialize(string value, Type type)
        {
            return JsonSerializer.Deserialize(value, type);
        }
    }
}