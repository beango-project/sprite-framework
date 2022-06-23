using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ImmediateReflection;
using JetBrains.Annotations;

namespace Sprite.Modular
{
    public class ModuleScanner : IModuleScanner
    {
        internal static ConcurrentDictionary<Type, ModuleConfig?> _moduleAndConfigMapsCache = new ConcurrentDictionary<Type, ModuleConfig?>();

        public List<Type> FindModuleTypes(Type tModuleType)
        {
            var moduleTypes = new List<Type>();
            AddModulesDndResolveDependencies(moduleTypes, tModuleType);
            return moduleTypes;
        }

        public List<Type> FindModuleDepends(Type tModuleType)
        {
            var dependencies = new List<Type>();
            var moduleImmediateType = tModuleType.GetImmediateType();
            var moduleConfig = FindModuleConfig(moduleImmediateType);
            if (moduleConfig != null)
            {
                foreach (var depended in moduleConfig.ImportModules())
                {
                    dependencies.AddIfNotContains(depended);
                }
            }

            return dependencies;
        }

        public List<Type> FindModuleDepends(Type tModuleType, out ModuleConfig config)
        {
            var dependencies = new List<Type>();
            var moduleImmediateType = tModuleType.GetImmediateType();
            var moduleConfig = FindModuleConfig(moduleImmediateType);
            if (moduleConfig != null)
            {
                moduleConfig.Configure(); //调用导入模块方法
                foreach (var depended in moduleConfig.ImportModules())
                {
                    dependencies.AddIfNotContains(depended);
                }
            }

            config = null;

            return dependencies;
        }


        [CanBeNull]
        public ModuleConfig FindModuleConfig(Type tModuleType)
        {
            return FindModuleConfig(tModuleType.GetImmediateType());
        }

        [CanBeNull]
        private ModuleConfig FindModuleConfig(ImmediateType tModuleType)
        {
            //如果没有从缓存中命中，则使用反射获取实例化并调用Configure()方法
            if (!_moduleAndConfigMapsCache.TryGetValue(tModuleType.Type, out var moduleConfig))
            {
                var attribute = tModuleType.GetAttribute<UsageAttribute>();
                if (attribute != null)
                {
                    var moduleConfigType = attribute.ModuleConfig;
                    if (moduleConfigType.IsClass && !moduleConfigType.IsAbstract && !moduleConfigType.IsGenericType && typeof(ModuleConfig).IsAssignableFrom(moduleConfigType))
                    {
                        moduleConfig = (ModuleConfig)moduleConfigType.New();
                        moduleConfig.Configure(); //调用导入模块方法
                        _moduleAndConfigMapsCache[tModuleType.Type] = moduleConfig;
                        return moduleConfig;
                    }
                }
            }

            return moduleConfig;
        }

        private void AddModulesDndResolveDependencies(List<Type> moduleTypes, Type tModuleType)
        {
            SpriteModule.CheckIsCandidate(tModuleType);

            if (moduleTypes.Contains(tModuleType))
            {
                return;
            }

            moduleTypes.Add(tModuleType);

            foreach (var depend in FindModuleDepends(tModuleType))
            {
                AddModulesDndResolveDependencies(moduleTypes, depend);
            }
        }
    }
}