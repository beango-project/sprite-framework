using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.Context;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    public class SpriteConventionControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly ISpriteApplicationContext _context;

        public SpriteConventionControllerFeatureProvider(ISpriteApplicationContext context)
        {
            _context = context;
        }

        protected override bool IsController(TypeInfo typeInfo)
        {
            if (_context.ServiceProvider == null)
            {
                return false;
            }

            var description = _context.ServiceProvider
                .GetRequiredService<IOptions<AspNetCoreMvcOptions>>().Value
                .Controllers
                .ControllerDescriptions
                .FirstOrDefault(x => x.ControllerTypes.Contains(typeInfo));

            return description != null;
        }
    }
}