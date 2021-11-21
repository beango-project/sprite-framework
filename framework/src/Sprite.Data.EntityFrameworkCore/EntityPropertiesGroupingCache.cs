using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImTools;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Sprite.Data.EntityFrameworkCore
{
    internal static class EntityPropertiesGroupingCache
    {
        private static ImHashMap<Type, List<KV<string, PropertyInfo[]>>> _map = ImHashMap<Type, List<KV<string, PropertyInfo[]>>>.Empty;

        public static void Add(Type type, string group, PropertyInfo[] propertyInfos)
        {
            Check.NotNull(group, nameof(group));

            var kv = new KV<string, PropertyInfo[]>(@group, propertyInfos);
            if (!_map.TryFind(type, out var maps)) //如果类不存在，则不添加类并添加分组属性关系
            {
                var list = new List<KV<string, PropertyInfo[]>>();
                list.Add(kv);
                _map = _map.AddOrUpdate(type, list);
            }
            else
            {
                if (maps.Find(x => x.Key == kv.Key) == null) //如果分组关系不存在则添加
                {
                    maps.Add(kv);
                }
            }
        }

        public static PropertyInfo[] Get(Type type, string group)
        {
            Check.NotNull(group, nameof(group));
            if (_map.TryFind(type, out var maps))
            {
                var gKv = maps.FirstOrDefault(x => x.Key == group);
                return gKv.Value;
            }

            return null;
        }
        
        public static (string, PropertyInfo[])? GetMap(Type type, string group)
        {
            Check.NotNull(group, nameof(group));
            if (_map.TryFind(type, out var maps))
            {
                var gKv = maps.FirstOrDefault(x => x.Key == group);
                return (gKv.Key, gKv.Value);
            }

            return null;
        }
        
    }
}