using System;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sprite.Modular;

namespace Sprite.ObjectMapping.Mapster
{
    public class SpriteMapsterModule : Module
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Describe(typeof(IObjectMapper), typeof(MapsterObjectMapper), ServiceLifetime.Singleton));
            services.AddSingleton(CreateMapperAccessor);
            services.AddSingleton<IMapperAccessor>(sp => sp.GetRequiredService<MapperAccessor>());
        }

        private MapperAccessor CreateMapperAccessor(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<MapsterOptions>>().Value;
                return new MapperAccessor
                {
                    Mapper = new Mapper(options.Config)
                };
            }
        }
    }
}