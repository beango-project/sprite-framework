using System;
using System.Linq.Expressions;
using System.Reflection;
using ImTools;

namespace Sprite.Data.EntityFrameworkCore
{
    internal static class EntityChangePropertiesCache
    {
        private static ImHashMap<Type, PropertyInfo[]> _map = ImHashMap<Type, PropertyInfo[]>.Empty;

        public static void Add(Type type, PropertyInfo[] propertyInfos)
        {
            if (!_map.Contains(type))
            {
                _map = _map.AddOrUpdate(type, propertyInfos);
            }
        }
        
        public static PropertyInfo[] Get(Type type)
        {
            if (_map.TryFind(type, out var propertyInfos))
            {
                return propertyInfos;
            }

            return null;
        }
    }
}