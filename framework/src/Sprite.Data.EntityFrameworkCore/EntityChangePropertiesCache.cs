using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ImTools;
using JetBrains.Annotations;

namespace Sprite.Data.EntityFrameworkCore
{
    //这个就是一个字典而已
    internal static class EntityChangePropertiesCache
    {
        // private static Ref<ImHashMap<Type, EntityChangePropertyEntry[]>> _cache = new Ref<ImHashMap<Type, EntityChangePropertyEntry[]>>();
        private static ImHashMap<Type, EntityChangePropertyEntry[]> _map = ImHashMap<Type, EntityChangePropertyEntry[]>.Empty;

        public static void Add(Type type, EntityChangePropertyEntry[] entityChangePropertyEntryList)
        {
            if (!_map.Contains(type))
            {
                _map = _map.AddOrUpdate(type, entityChangePropertyEntryList);
            }
        }

        [CanBeNull]
        public static EntityChangePropertyEntry[] Get(Type type)
        {
            if (_map.TryFind(type, out var list))
            {
                return list;
            }

            return null;
        }

        public static void Clear()
        {
            _map = null;
        }

        public static IDictionary<Type, EntityChangePropertyEntry[]> ToDictionary() => _map.ToDictionary();
    }
}