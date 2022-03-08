using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Extensions.Logging;
using Sprite.DependencyInjection;
using Sprite.Modular;

namespace Sprite.Context
{
    public abstract class SpriteApplicationContextBase : ISpriteApplicationContext
    {
        private readonly IModuleStore _moduleStore;

        private ILogger<SpriteApplicationContextBase> _logger;

        public SpriteApplicationContextBase([NotNull] Type rootModuleType, [NotNull] IServiceCollection services, [CanBeNull] Action<SpriteApplicationCreateOptions>
            options = null)
        {
            Check.NotNull(rootModuleType, nameof(rootModuleType));
            Check.NotNull(services, nameof(services));
            RootModuleType = rootModuleType;
            Services = services;
       
            var defaultOptions = new SpriteApplicationCreateOptions(services);
            options?.Invoke(defaultOptions);


            services.TryAddSwapSpace();
            services.TryAddInSwapSpace<IServiceProvider>();


            services.TryAddSingleton<ISpriteApplicationContext>(this);

            services.AddCoreService();
            services.AddSpriteService(rootModuleType, defaultOptions);


            var modules = LoadModules(services);
            _moduleStore = GetAndSetModuleStore(modules);
        }


        public Type RootModuleType { get; }
        public IServiceCollection Services { get; }
        public IServiceProvider ServiceProvider { get; private set; }

        public virtual void Dispose()
        {
        }

        public virtual void Shutdown()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                scope.ServiceProvider
                    .GetRequiredService<IModuleManager>()
                    .ShutdownModules(new OnApplicationContext(scope.ServiceProvider));
            }
        }

        private IModuleStore GetAndSetModuleStore(List<IModuleDefinition> moduleDefinitions)
        {
            var moduleStore = Services.GetRequestSingletonInstance<IModuleStore>();
            // var moduleStore = new ModuleStore();
            foreach (var moduleDefinition in moduleDefinitions)
            {
                moduleStore.Add(moduleDefinition);
            }

            return moduleStore;
        }

        private List<IModuleDefinition> LoadModules(IServiceCollection services)
        {
            var moduleLoader = services.GetRequestSingletonInstance<IModuleLoader>();
            return moduleLoader.LoadModules();
        }

        protected virtual void Initialize()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                // var logger = scope.ServiceProvider.GetService<ILogger<SpriteApplicationContextBase>>();
                // if (logger == null)
                // {
                //     return;
                // }
                //
                // var initLogger =  scope.ServiceProvider.GetRequiredService<IInitLoggerFactory>().Create<SpriteApplicationContextBase>();
                //
                // foreach (var entry in initLogger.Entries)
                // {
                //     logger.LogDebug(entry.Message);
                // }

                // initLogger.Entries.Clear();

                scope.ServiceProvider.GetRequiredService<IModuleManager>().InitializeModules(new OnApplicationContext(scope.ServiceProvider));
            }
        }

        public virtual void ShutdownModules()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IModuleManager>().ShutdownModules(new OnApplicationContext(scope.ServiceProvider));
            }
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<SwapSpace>().Set<IServiceProvider>(ServiceProvider);
            ApplicationServices.Initialize(serviceProvider);
        }
    }
}