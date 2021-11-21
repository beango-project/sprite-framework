using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Sprite.Modular
{
    public class ModuleDefinition : IModuleDefinition
    {
        private readonly HashSet<IModuleDefinition> _dependModules;

        private readonly HashSet<IModuleProcessor> _processors;

        public ModuleDefinition(Type module, IModule moduleInstance, bool isSkipAutoScanRegister = false, HashSet<IModuleProcessor> processors = null)
        {
            Check.NotNull(module, nameof(module));
            Check.NotNull(moduleInstance, nameof(moduleInstance));

            Module = module;
            ModuleInstance = moduleInstance;
            IsSkipAutoScanRegister = isSkipAutoScanRegister;
            _processors = processors ?? new HashSet<IModuleProcessor>();
            _dependModules = new HashSet<IModuleDefinition>();
        }

        public Type Module { get; }
        public IModule ModuleInstance { get; }
        public IReadOnlySet<IModuleDefinition> DependModules => _dependModules.ToImmutableHashSet();
        public IReadOnlySet<IModuleProcessor> Processors => _processors.ToImmutableHashSet();
        public bool IsSkipAutoScanRegister { get; }

        public void AddDependModules(IModuleDefinition moduleDefinition)
        {
            if (!_dependModules.Contains(moduleDefinition))
            {
                _dependModules.Add(moduleDefinition);
            }
        }
    }
}