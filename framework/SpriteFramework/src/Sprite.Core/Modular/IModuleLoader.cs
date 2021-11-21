using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sprite.Modular
{
    public interface IModuleLoader
    {
        [NotNull]
        List<IModuleDefinition> LoadModules();
    }
}