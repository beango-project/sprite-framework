using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sprite.DependencyInjection
{
    public class ExportServiceArgs
    {
        public ExportServiceArgs([NotNull] Type implementationType, List<Type> exportTypes)
        {
            ImplementationType = Check.NotNull(implementationType, nameof(implementationType));
            ExportTypes = Check.NotNull(exportTypes, nameof(exportTypes));
        }

        public Type ImplementationType { get; }

        public List<Type> ExportTypes { get; }
    }
}