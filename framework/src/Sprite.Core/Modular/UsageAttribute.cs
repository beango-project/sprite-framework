using System;

namespace Sprite.Modular
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsageAttribute : Attribute
    {
        public UsageAttribute(Type moduleConfig)
        {
            ModuleConfig = moduleConfig;
        }

        public Type ModuleConfig { get; }
    }
}