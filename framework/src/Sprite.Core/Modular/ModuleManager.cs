using System;
using System.Linq;
using AspectCore.Extensions.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sprite.DependencyInjection;

namespace Sprite.Modular
{
    public class ModuleManager : IModuleManager, ISingletonInjection
    {
        public const string ModuleConfigureName = "Configure";

        private readonly ILogger<ModuleManager> _logger;

        private readonly IModuleStore _moduleStore;

        [CanBeNull]
        private ISwapSpace _swapSpace;

        public ModuleManager(IModuleStore moduleStore, ILogger<ModuleManager> logger)
        {
            _moduleStore = moduleStore;
            _logger = logger;
        }


        public void InitializeModules(OnApplicationContext context)
        {
            LoadSwapSpace(context);
            foreach (var module in _moduleStore.ModuleMaps)
            {
                ProcessModuleConfigure(context, module);
            }
        }

        public void ShutdownModules(OnApplicationContext context)
        {
            foreach (var module in _moduleStore.ModuleMaps)
            {
                var processors = module.Processors;
                foreach (var shutdownProcessor in processors.OfType<IModuleShutdownProcessor>().OrderBy(p => p.Order))
                {
                    shutdownProcessor.Shutdown(context);
                }
            }
        }

        private void ProcessModuleConfigure(OnApplicationContext context, IModuleDefinition module)
        {
            var moduleTypeInfo = module.ModuleInstance.GetType().GetReflector().GetMemberInfo();
            var configureMethod = moduleTypeInfo.GetMethod(ModuleConfigureName);
            if (configureMethod == null)
            {
                return;
            }

            var memberInfo = configureMethod.GetReflector();
            var isCandidate = configureMethod.ReturnType == typeof(void) && !configureMethod.IsStatic && !configureMethod.IsAbstract && !configureMethod.IsConstructor;
            if (isCandidate)
            {
                var parameters = memberInfo.ParameterReflectors;
                var methodsParameters = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].HasDeflautValue)
                    {
                        methodsParameters[i] = parameters[i].DefalutValue;
                        continue;
                    }

                    try
                    {
                        _swapSpace.TryGet(parameters[i].ParameterType, out var findValue);
                        // If it is not found in the swap area, it will be resolved in the container
                        methodsParameters[i] = findValue ?? context.ServiceProvider.GetRequiredService(parameters[i].ParameterType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Could not resolve a type :{0} for the parameter '{1}'of method'{2}' on type '{3}'.",
                            parameters[i].ParameterType.FullName, parameters[i].Name, configureMethod.Name, configureMethod.DeclaringType?.FullName), ex);
                    }
                }

                memberInfo.Invoke(module.ModuleInstance, methodsParameters);
            }
        }

        private void LoadSwapSpace(OnApplicationContext context)
        {
            try
            {
                _swapSpace = context.ServiceProvider.GetRequiredService<SwapSpace>();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not resolve a type :{0}.", typeof(SwapSpace), ex));
            }
        }
    }
}