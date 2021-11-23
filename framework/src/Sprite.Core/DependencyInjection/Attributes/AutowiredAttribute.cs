using System;
using JetBrains.Annotations;

namespace Sprite.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AutowiredAttribute : Attribute
    {
        [CanBeNull]
        public Type Type { get; set; }

        [CanBeNull]
        public object Key { get; set; }

        public AutowiredAttribute()
        {
        }

        public AutowiredAttribute(Type type)
        {
            Type = type;
        }
    }
}