using System;

namespace Sprite.Modular
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsageAttribute : Attribute
    {
        public UsageAttribute(Type moduleConfigure)
        {
            ModuleConfigure = moduleConfigure;
        }

        public Type ModuleConfigure { get; }
    }
}