using System;
using JetBrains.Annotations;

namespace Sprite.Modular
{
    public abstract class ModuleConfig
    {
        protected internal bool SkipAutoScanRegister = false;
        private Type[] _dependedModules;

        [NotNull]
        public Type[] DependedModules => _dependedModules;

        protected void ImportModules(params Type[] dependedModules)
        {
            _dependedModules = dependedModules ?? Type.EmptyTypes;
        }

        public abstract void Configure();
    }
}