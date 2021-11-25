using System;
using JetBrains.Annotations;

namespace Sprite.Modular
{
    public abstract class ModuleConfig
    {
        protected internal bool SkipAutoScanRegister = false;

        [NotNull]
        public Type[] DependedModules { get; private set; }

        protected void ImportModules(params Type[] dependedModules)
        {
            DependedModules = dependedModules ?? new Type[0];
        }

        public abstract void Configure();
    }
}