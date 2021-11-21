using System;

namespace Sprite.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AutowiredAttribute : Attribute
    {
        public Type Type { get; set; }

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