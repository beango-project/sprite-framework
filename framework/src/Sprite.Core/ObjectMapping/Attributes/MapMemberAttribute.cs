using System;

namespace Sprite.ObjectMapping.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class MapMemberAttribute : Attribute
    {
        public string? Name { get; set; }

        public Mapping? Mapping { get; set; }

        public MapMemberAttribute()
        {
        }

        public MapMemberAttribute(string name)
        {
            Name = name;
        }

        public MapMemberAttribute(string name, Mapping mapping)
        {
            Name = name;
            Mapping = mapping;
        }
    }
}