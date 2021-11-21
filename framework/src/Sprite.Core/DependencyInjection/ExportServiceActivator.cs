using System;
using System.Collections.Generic;

namespace Sprite.DependencyInjection
{
    public class ExportServiceActivator : List<Action<ExportServiceArgs>>
    {
    }
}