using System;

namespace Sprite.Modular
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsageOnAttribute : Attribute
    {
        public UsageOnAttribute(Type moduleConfigure)
        {
            ModuleConfigure = moduleConfigure;
        }

        public Type ModuleConfigure { get; }
    }
}