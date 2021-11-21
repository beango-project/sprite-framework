using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sprite.AspNetCore.Mvc.AntiForgery;
using Sprite.AspNetCore.Mvc.ApiExplorer;
using Sprite.AspNetCore.Mvc.Conventions;
using Sprite.AspNetCore.Mvc.ExceptionHandles;
using Sprite.AspNetCore.Mvc.Results;
using Sprite.AspNetCore.Mvc.Uow;
using Sprite.Context;
using Sprite.Data.Transaction;
using Sprite.Modular;
using Sprite.Security.Authorization;

namespace Sprite.AspNetCore.Mvc
{
    [Usage(typeof(SpriteAspNetCoreMvcConfigure))]
    public class SpriteAspNetCoreMvcModule : Module
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            AddAndConfigureRemoteApiServices(services);

            services.Configure<AspNetCoreMvcOptions>(options => { options.NormalizerResult = true; });
            // Configure Mvc core services 
            ConfigureMvcCoreServices(services);
        }

        public void Configure(ApplicationPartManager partManager, IOptions<AspNetCoreMvcOptions> options)
        {
            AddControllersInApplicationPart(partManager, options);
        }

        /// <summary>
        /// 在ApplicationPart添加控制器
        /// </summary>
        /// <param name="applicationPartManager"></param>
        /// <param name="options"></param>
        private void AddControllersInApplicationPart(ApplicationPartManager applicationPartManager, IOptions<AspNetCoreMvcOptions> options)
        {
            if (applicationPartManager == null)
            {
                return;
            }

            var assemblies = options?.Value.Controllers.ControllerDescriptions.Select(x => x.FromAssembly).Distinct();
            foreach (var assembly in assemblies)
            {
                applicationPartManager.ApplicationParts.AddIfNotContains(assembly);
            }
        }

        private void AddAndConfigureRemoteApiServices(IServiceCollection services)
        {
            services.Configure<RemoteApiDescriptionProviderOptions>(options =>
            {
                var statusCodes = new List<int>
                {
                    (int)HttpStatusCode.Forbidden,
                    (int)HttpStatusCode.Unauthorized,
                    (int)HttpStatusCode.BadRequest,
                    (int)HttpStatusCode.NotFound,
                    (int)HttpStatusCode.NotImplemented,
                    (int)HttpStatusCode.InternalServerError,
                    (int)HttpStatusCode.OK
                };
                //
                // var statusCodes = new List<int>();
                // statusCodes.AddRange(Enum.GetValues<HttpStatusCode>().Cast<int>().Select(x =>{100,200});

                //Possible type of the response body which is formatted by ApiResponseFormats.
                options.SupportedResponseTypes.AddIfNotContains(statusCodes.Select(statusCode => new ApiResponseType
                {
                    Type = typeof(RestNormalizedResultResponse),
                    StatusCode = statusCode
                }));
            });
        }

        private IMvcCoreBuilder ConfigureMvcCoreServices(IServiceCollection services)
        {
            var mvcCoreBuilder = services.AddMvcCore(options =>
            {
                options.Filters.Add<SpriteAutoValidateAntiForgeryTokenAttribute>();
                options.Filters.AddService<UnitOfWorkActionFilter>();
                options.Filters.AddService(typeof(SpriteResultFilter));
                options.Filters.AddService(typeof(SpriteExceptionFilter));
            });

            services.TryAddTransient<AutoValidateAntiforgeryTokenAttribute>();

            // Use DI create mvc services 
            var mvcBuilder = services.AddMvc().AddRazorRuntimeCompilation();
            //Use DI to create controllers
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //Use DI to create view components
            services.Replace(ServiceDescriptor.Singleton<IViewComponentActivator, ServiceBasedViewComponentActivator>());

            //Use DI to create razor page
            services.Replace(ServiceDescriptor.Singleton<IPageModelActivatorProvider, ServiceBasedPageModelActivatorProvider>());

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            var partManager = services.GetSingletonInstance<ApplicationPartManager>();

            if (partManager == null)
            {
                throw new InvalidOperationException("Could not find singleton service: " + typeof(ApplicationPartManager).AssemblyQualifiedName);
            }

            var context = services.GetSingletonInstance<ISpriteApplicationContext>();

            // Add a custom controller checker
            partManager.FeatureProviders.Add(new SpriteConventionControllerFeatureProvider(context));
            partManager.ApplicationParts.AddIfNotContains<SpriteAspNetCoreMvcModule>();


            services.Configure<MvcOptions>(options =>
            {
                // Register Controller Routing Information Converter
                options.Conventions.Add(new AppServiceModelConvention(services));
            });

            return mvcCoreBuilder;
        }
    }
}