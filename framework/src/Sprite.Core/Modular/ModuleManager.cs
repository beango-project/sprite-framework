using System;
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
                foreach (var processor in processors)
                {
                    if (processor is IModuleShutdownProcessor shutdownProcessor)
                    {
                        shutdownProcessor.Shutdown(context);
                    }
                }
            }
        }

        private void ProcessModuleConfigure(OnApplicationContext context, IModuleDefinition module)
        {
            var configureMethod = module.SpriteModuleInstance.GetType().GetMethod(ModuleConfigureName);
            if (configureMethod == null)
            {
                return;
            }

            var isCandidate = configureMethod.ReturnType == typeof(void) && !configureMethod.IsStatic && !configureMethod.IsAbstract && !configureMethod.IsConstructor;
            if (isCandidate)
            {
                var parameters = configureMethod.GetParameters();
                var methodsParameters = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].HasDefaultValue)
                    {
                        methodsParameters[i] = parameters[i].DefaultValue;
                        continue;
                    }

                    try
                    {
                        var findValue = _swapSpace.TryGet(parameters[i].ParameterType) ?? context.ServiceProvider.GetRequiredService(parameters[i].ParameterType);

                        methodsParameters[i] = findValue;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Could not resolve a type :{0} for the parameter '{1}'of method'{2}' on type '{3}'.",
                            parameters[i].ParameterType.FullName, parameters[i].Name, configureMethod.Name, configureMethod.DeclaringType?.FullName), ex);
                    }
                }

                configureMethod.Invoke(module.SpriteModuleInstance, methodsParameters);
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