using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Sprite.Modular
{
    public class ModuleDefinition : IModuleDefinition
    {
        public Type Module { get; }
        public IModule ModuleInstance { get; }

        public IReadOnlySet<IModuleDefinition> DependModules => _dependModules.ToImmutableHashSet();

        private readonly HashSet<IModuleDefinition> _dependModules;

        public ModuleDefinition(Type module, IModule moduleInstance)
        {
            Guard.CheckNotNull(module, nameof(module));
            Guard.CheckNotNull(moduleInstance, nameof(moduleInstance));

            Module = module;
            ModuleInstance = moduleInstance;
            this._dependModules = new HashSet<IModuleDefinition>();
        }

        public void AddDependModules(IModuleDefinition moduleDefinition)
        {
            if (!this._dependModules.Contains(moduleDefinition))
            {
                this._dependModules.Add(moduleDefinition);
            }
        }
    }
}