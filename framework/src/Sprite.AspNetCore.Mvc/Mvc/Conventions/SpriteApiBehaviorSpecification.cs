using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    public class SpriteApiBehaviorSpecification : IApiControllerSpecification
    {
        private readonly AspNetCoreMvcOptions _options;

        public SpriteApiBehaviorSpecification(IOptions<AspNetCoreMvcOptions> options)
        {
            _options = options.Value;
        }

        public bool IsSatisfiedBy(ControllerModel controller)
        {
            var controllerDescriptionModel = _options.Controllers
                .ControllerDescriptions
                .FirstOrDefault(x =>
                    x.ControllerTypes.Contains(controller.ControllerType.AsType())
                );
            return controllerDescriptionModel != null;
        }
    }
}