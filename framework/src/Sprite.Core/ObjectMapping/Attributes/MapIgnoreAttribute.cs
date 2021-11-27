using System;
using JetBrains.Annotations;

namespace Sprite.ObjectMapping.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MapIgnoreAttribute : Attribute
    {
        public Mapping? Mapping { get; set; }


        public MapIgnoreAttribute()
        {
        }

        public MapIgnoreAttribute(Mapping mapping)
        {
            Mapping = mapping;
        }
    }
}