using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Data.Repositories;
using Sprite.DependencyInjection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Data.DependencyInjection
{
    public class SpriteRepositoryRegistrationRules : DefaultRegistrationRules
    {
        public override void AddFromTypeOf(IServiceCollection services, Type type)
        {
            if (!typeof(IRepository).IsAssignableFrom(type))
            {
                return;
            }

            base.AddFromTypeOf(services, type);
        }

        protected override List<Type> GetExportServiceTypes(Type type)
        {
            return base.GetExportServiceTypes(type).Where(x => x.IsInterface).ToList();
        }

        protected override ServiceLifetime? GetLifeTime(Type type, RegisterAttribute registerAttribute)
        {
            return base.GetLifeTime(type, registerAttribute) ?? ServiceLifetime.Transient;
        }
    }
}