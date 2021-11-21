using System;
using System.Collections.Generic;

namespace Sprite.Sessions
{
    public class NullSession : ICurrentSession
    {
        public static NullSession Instance => new();

        public string Id { get; }

        public bool IsAvailable { get; }

        public string ChangeSessionId { get; }

        public IEnumerable<string> Keys { get; }

        public bool TryGetValue(string key, out byte[] value)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}