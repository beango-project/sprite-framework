using System.Collections.Generic;

namespace Sprite.Modular
{
    public interface IModuleStore
    {
        IReadOnlyList<IModuleDefinition> ModuleMaps { get; }
        void Add(IModuleDefinition moduleDefinition);
        void Remove(IModuleDefinition moduleDefinition);
        ICollection<IModuleDefinition> GetModuleDefinitions();
    }
}