using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Sprite.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringAttribute : Attribute
    {
        [NotNull] public string Name { get; }

        public ConnectionStringAttribute([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));
            Name = name;
        }

        [CanBeNull]
        public static string GetConnectionStringName<T>()
        {
            return GetConnectionStringName(typeof(T));
        }


        [CanBeNull]
        public static string GetConnectionStringName(Type type)
        {
            var connectionStringAttribute = type.GetTypeInfo().GetAttributeWithDefined<ConnectionStringAttribute>();
            if (connectionStringAttribute == null)
            {
                return null;
            }

            return connectionStringAttribute.Name;
        }
    }
}