using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Sprite.Modular
{
    public class ModuleStore : IModuleStore
    {
        private readonly List<IModuleDefinition> _moduleDefinitions;

        public ModuleStore()
        {
            _moduleDefinitions = new List<IModuleDefinition>();
        }

        public IReadOnlyList<IModuleDefinition> ModuleMaps => _moduleDefinitions.ToImmutableList();

        public void Add(IModuleDefinition moduleDefinition)
        {
            _moduleDefinitions.Add(moduleDefinition);
        }

        public void Remove(IModuleDefinition moduleDefinition)
        {
            _moduleDefinitions.Remove(moduleDefinition);
        }

        [CanBeNull]
        public ICollection<IModuleDefinition> GetModuleDefinitions()
        {
            return _moduleDefinitions;
        }
    }
}