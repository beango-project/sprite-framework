using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Sprite.Caching
{
    public class CacheHeatHolder : ConcurrentDictionary<string, int>
    {
        public string GetTopHotKey()
        {
            return this.OrderByDescending(x => x.Value).FirstOrDefault().Key;
        }

        public string[] GetTopHotKey(int size)
        {
            return this.OrderByDescending(x => x.Value).Take(size).Select(x => x.Key).ToArray();
        }

        public void AddHeatValue(string cacheKey)
        {
            AddOrUpdate(cacheKey, _ => 0, (k, v) =>
            {
                var valueOrDefault = this.GetValueOrDefault(k);
                return valueOrDefault++;
            });
        }
    }
}