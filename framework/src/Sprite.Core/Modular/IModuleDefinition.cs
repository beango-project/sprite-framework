using System;
using System.Collections.Generic;

namespace Sprite.Modular
{
    /// <summary>
    /// 模块定义
    /// </summary>
    public interface IModuleDefinition
    {
        Type Module { get; }

        ISpriteModule ModuleInstance { get; }

        IReadOnlySet<IModuleDefinition> DependModules { get; }

        IReadOnlySet<IModuleProcessor> Processors { get; }

        bool IsSkipAutoScanRegister { get; }
    }
}