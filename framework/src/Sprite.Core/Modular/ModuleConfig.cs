using System;
using JetBrains.Annotations;

namespace Sprite.Modular
{
    public abstract class ModuleConfig
    {
        protected internal bool SkipAutoScanRegister = false;

        public virtual Type[] ImportModules()
        {
            return Type.EmptyTypes;
        }

        public virtual void Configure()
        {
        }
    }
}