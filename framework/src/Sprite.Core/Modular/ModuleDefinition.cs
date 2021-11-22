using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Sprite.Modular
{
    public class ModuleDefinition : IModuleDefinition
    {
        private readonly HashSet<IModuleDefinition> _dependModules;

        private readonly HashSet<IModuleProcessor> _processors;

        public ModuleDefinition(Type module, ISpriteModule spriteModuleInstance, bool isSkipAutoScanRegister = false, HashSet<IModuleProcessor> processors = null)
        {
            Check.NotNull(module, nameof(module));
            Check.NotNull(spriteModuleInstance, nameof(spriteModuleInstance));

            Module = module;
            SpriteModuleInstance = spriteModuleInstance;
            IsSkipAutoScanRegister = isSkipAutoScanRegister;
            _processors = processors ?? new HashSet<IModuleProcessor>();
            _dependModules = new HashSet<IModuleDefinition>();
        }

        public Type Module { get; }
        public ISpriteModule SpriteModuleInstance { get; }
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