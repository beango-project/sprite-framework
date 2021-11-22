using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Sprite.Modular
{
    public class ModuleScanner : IModuleScanner
    {
        public List<Type> FindModuleTypes(Type tModuleType)
        {
            var moduleTypes = new List<Type>();
            AddModulesDndResolveDependencies(moduleTypes, tModuleType);
            return moduleTypes;
        }

        public List<Type> FindModuleDepends(Type tModuleType)
        {
            var dependencies = new List<Type>();
            var configureType = FindModuleConfigure(tModuleType);
            if (configureType != null)
            {
                var moduleConfigure = (ModuleConfigure) Activator.CreateInstance(configureType)!;
                moduleConfigure.Configure(); //调用导入模块方法
                foreach (var depended in moduleConfigure.DependedModules)
                {
                    dependencies.AddIfNotContains(depended);
                }
            }

            return dependencies;
        }


        [CanBeNull]
        public Type FindModuleConfigure(Type tModuleType)
        {
            var attribute = tModuleType.GetCustomAttribute<UsageAttribute>();
            if (attribute != null)
            {
                // var moduleConfigure = tModuleType.Assembly.GetTypes()
                //     .FirstOrDefault(x =>
                //         x.IsClass &&
                //         !x.IsAbstract &&
                //         !x.IsGenericType &&
                //         typeof(ModuleConfigure).IsAssignableFrom(x));
                var moduleConfigure = attribute.ModuleConfigure;
                if (moduleConfigure.IsClass && !moduleConfigure.IsAbstract && !moduleConfigure.IsGenericType && typeof(ModuleConfigure).IsAssignableFrom(moduleConfigure))
                {
                    return moduleConfigure;
                }
            }

            return null;
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