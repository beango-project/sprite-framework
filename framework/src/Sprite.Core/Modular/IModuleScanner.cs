using System;
using System.Collections.Generic;

namespace Sprite.Modular
{
    /// <summary>
    /// 模块扫描器，用于扫描模块、查找模块的依赖
    /// </summary>
    public interface IModuleScanner
    {
        /// <summary>
        /// 查找模块类型
        /// </summary>
        /// <param name="tModuleType">模块类型</param>
        /// <returns></returns>
        List<Type> FindModuleTypes(Type tModuleType);

        /// <summary>
        /// 查找模块依赖
        /// </summary>
        /// <param name="tModuleType"></param>
        /// <returns></returns>
        List<Type> FindModuleDepends(Type tModuleType);
    }
}