using System;
using System.Collections.Generic;
using System.Linq;
using ImmediateReflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Modular
{
    public class ModuleLoader : IModuleLoader
    {
        private readonly Type _rootModuleType;

        private readonly IModuleScanner _scanner;
        private readonly IServiceCollection _services;

        public ModuleLoader(IServiceCollection services, Type rootModuleType)
        {
            Check.NotNull(services, nameof(services));
            _services = services;
            _rootModuleType = rootModuleType;
            _scanner = new ModuleScanner();
        }

        public ModuleLoader(IServiceCollection services, Type rootModuleType, IModuleScanner moduleScanner)
        {
            Check.NotNull(services, nameof(services));
            _services = services;
            _rootModuleType = rootModuleType;
            _scanner = moduleScanner;
        }

        public List<IModuleDefinition> LoadModules()
        {
            // logger.LogInformation("开始加载模块和定义");
            var moduleDefinitions = LoadModuleDefinitions();
            moduleDefinitions = SortImportModuleSetDependency(moduleDefinitions, _rootModuleType);
            ConfigureModuleServices(moduleDefinitions);

            return moduleDefinitions;
        }

        private void ConfigureModuleServices(List<IModuleDefinition> moduleDefinitions)
        {
            foreach (var moduleDefinition in moduleDefinitions)
            {
                if (moduleDefinition.ModuleInstance is SpriteModule)
                {
                    //TODO We can extract ConfigureServicesProcessors ?
                    var configureServicesProcessors = moduleDefinition.Processors.AsParallel().OfType<IConfigureServicesProcessor>().OrderBy(x => x.Order);

                    if (!moduleDefinition.IsSkipAutoScanRegister)
                    {
                        _services.AddFromAssemblyOf(moduleDefinition.Module.Assembly);
                    }

                    foreach (var configureServicesProcessor in configureServicesProcessors)
                    {
                        configureServicesProcessor.BeforeConfigureServices(_services);
                    }

                    moduleDefinition.ModuleInstance.ConfigureServices(_services);

                    foreach (var configureServicesProcessor in configureServicesProcessors)
                    {
                        configureServicesProcessor.AfterConfigureServices(_services);
                        // TODO:We can remove the used Processor to reduce memory usage?
                        // moduleDefinition.RemoveProcessor(configureServicesProcessor);
                    }
                }
            }
        }

        protected virtual List<IModuleDefinition> SortImportModuleSetDependency(List<IModuleDefinition> moduleDefinitions, Type rootModuleType)
        {
            //TODO 加载模块，并加载它的配置
            var sortedModules = moduleDefinitions.SortByDependencies(m => m.DependModules);
            sortedModules.MoveItem(x => x.Module == rootModuleType, moduleDefinitions.Count - 1);
            return sortedModules;
        }

        private List<IModuleDefinition> LoadModuleDefinitions()
        {
            var moduleDefinitions = new List<ModuleDefinition>();
            LoadAndBuildModules(moduleDefinitions);
            SetDependencies(moduleDefinitions);
            return moduleDefinitions.Cast<IModuleDefinition>().ToList();
        }

        private void LoadAndBuildModules(List<ModuleDefinition> moduleDefinitions)
        {
            var findModuleTypes = _scanner.FindModuleTypes(_rootModuleType);
            foreach (var findModuleType in findModuleTypes)
            {
                moduleDefinitions.Add(CreateModuleDefinition(findModuleType));
            }
        }

        private ModuleDefinition CreateModuleDefinition(Type moduleType)
        {
            var module = CreateAndAddInServicesModule(moduleType);
            var processors = LoadModuleProcessors(moduleType);
            var moduleConfig = _scanner.FindModuleConfig(moduleType);
            if (moduleConfig != null)
            {
                return new ModuleDefinition(moduleType, module, moduleConfig.SkipAutoScanRegister, processors);
            }

            return new ModuleDefinition(moduleType, module, processors: processors);
        }


        private ISpriteModule CreateAndAddInServicesModule(Type moduleType)
        {
            if (TypeAccessor.Get(moduleType).TryNew(out var module, out var exception))
            {
                _services.AddSingleton(moduleType, (ISpriteModule)module);
                return (ISpriteModule)module;
            }

            throw exception;
        }

        /// <summary>
        /// 加载此模块的所有模块处理器
        /// Load this module all module processor
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns></returns>
        private HashSet<IModuleProcessor> LoadModuleProcessors(Type moduleType)
        {
            var processorsList = moduleType.Assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType && typeof(IModuleProcessor).IsAssignableFrom(x));
            var processorSet = new HashSet<IModuleProcessor>();
            foreach (var processor in processorsList)
            {
                var instance = (IModuleProcessor)processor.New();
                processorSet.Add(instance);
            }

            return processorSet;
        }

        protected virtual void SetDependencies(List<ModuleDefinition> moduleDefinitions)
        {
            foreach (var module in moduleDefinitions)
            {
                SetDependencies(moduleDefinitions, module);
            }
        }

        protected virtual void SetDependencies(List<ModuleDefinition> moduleDefinitions, ModuleDefinition moduleDefinition)
        {
            foreach (var dependedModuleType in _scanner.FindModuleDepends(moduleDefinition.Module))
            {
                var dependedModule = moduleDefinitions.FirstOrDefault(m => m.Module == dependedModuleType);
                if (dependedModule == null)
                {
                    throw new Exception("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleDefinition.Module.AssemblyQualifiedName);
                }

                moduleDefinition.AddDependModules(dependedModule);
            }
        }
    }
}