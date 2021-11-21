using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Sprite.AspNetCore.Mvc;
using Sprite.AspNetCore.Mvc.Conventions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddSpriteApiVersioning(this IServiceCollection services, Action<ApiVersioningOptions> optionAction)
        {
            services.AddTransient<IApiControllerSpecification, SpriteApiBehaviorSpecification>();

            services.AddApiVersioning(optionAction);


            // var apiVersioningOptions = serviceProvider.GetRequiredService<IOptions<ApiVersioningOptions>>().Value;

            // Configure(aspNetCoreMvcOptions, apiVersioningOptions);


            return services;
        }

        public static void ApplyVersioning(this ApiVersioningOptions apiVersioningOptions, IServiceCollection services)
        {
            var mvcOptions = services.BuildServiceProviderFromFactory().GetRequiredService<IOptions<AspNetCoreMvcOptions>>().Value;
            foreach (var description in mvcOptions.Controllers.ControllerDescriptions)
            {
                if (description.ApiVersionOptions == null)
                {
                    ConfigureApiVersionsByConvention(apiVersioningOptions, description);
                }
                else
                {
                    description.ApiVersionOptions.Invoke(apiVersioningOptions);
                }
            }
        }

        private static void ConfigureApiVersionsByConvention(ApiVersioningOptions options, ConventionControllerDescription description)
        {
            foreach (var controllerType in description.ControllerTypes)
            {
                var controllerBuilder = options.Conventions.Controller(controllerType);

                if (description.ApiVersions.Any())
                {
                    foreach (var apiVersion in description.ApiVersions)
                    {
                        controllerBuilder.HasApiVersion(apiVersion);
                    }
                }
                else
                {
                    if (!controllerType.IsDefined(typeof(ApiVersionAttribute), true))
                    {
                        controllerBuilder.IsApiVersionNeutral();
                    }
                }
            }
        }
    }
}