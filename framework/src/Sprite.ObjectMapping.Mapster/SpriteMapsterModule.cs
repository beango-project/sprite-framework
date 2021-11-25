using System;
using FastExpressionCompiler;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sprite.Modular;
using Sprite.ObjectMapping.Mapster.Attributes;

namespace Sprite.ObjectMapping.Mapster
{
    public class SpriteMapsterModule : SpriteModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {

            MapsterConfiguration();
            
            services.Add(ServiceDescriptor.Describe(typeof(IObjectMapper), typeof(MaptserObjectMapper), ServiceLifetime.Singleton));
            services.AddSingleton(CreateMapper);
            services.AddSingleton<IMapper>(sp => sp.GetRequiredService<Mapper>());
            // services.AddSingleton<IMapperAccessor>(sp => sp.GetRequiredService<MapsterMapperAccessor>());
        }

        private void MapsterConfiguration()
        {
            //FEC
            TypeAdapterConfig.GlobalSettings.Compiler = expression => expression.CompileFast();
            TypeAdapterConfig.GlobalSettings.Default.Config.CompileProjection();
            TypeAdapterConfig.GlobalSettings.Default.IgnoreAttribute(typeof(NotMapAttribute));
        }
        
        private Mapper CreateMapper(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<MapsterOptions>>().Value;
                
                // Validate
                options.GlobalSettings.Compile();
                
                return new Mapper();
            }
        }

        // private MapsterMapperAccessor CreateMapperAccessor(IServiceProvider serviceProvider)
        // {
        //     using (var scope = serviceProvider.CreateScope())
        //     {
        //         var options = scope.ServiceProvider.GetRequiredService<IOptions<MapsterOptions>>().Value;
        //         options.GlobalSettings.Compile();
        //         return new MapsterMapperAccessor
        //         {
        //             
        //             Mapper = new Mapper(options.GlobalSettings.Clone())
        //         };
        //     }
        // }
    }
}