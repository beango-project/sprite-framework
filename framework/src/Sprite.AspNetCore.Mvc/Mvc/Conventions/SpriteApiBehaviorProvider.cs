using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    [Component(ServiceLifetime.Transient)]
    public class SpriteApiBehaviorProvider
    {
        public SpriteApiBehaviorProvider(IOptions<AspNetCoreMvcOptions> mvcOptions, IOptions<ApiVersioningOptions> apiOptions)
        {
            Configure(mvcOptions?.Value, apiOptions?.Value);
        }

        private void Configure(AspNetCoreMvcOptions mvcOptions, ApiVersioningOptions apiVersioningOptions)
        {
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

        private void ConfigureApiVersionsByConvention(ApiVersioningOptions options, ConventionControllerDescription description)
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