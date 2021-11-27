using System;
using FastExpressionCompiler;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sprite.Modular;
using Sprite.ObjectMapping.Attributes;

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
            
            TypeAdapterConfig.GlobalSettings.Default.Settings.GetMemberNames.Add(((model, side) =>
            {
                var mapMemberAttr = model.GetCustomAttribute<MapMemberAttribute>();
                if (mapMemberAttr == null)
                {
                    return null;
                }


#if (NETCOREAPP3_1_OR_GREATER)

                return mapMemberAttr.Mapping switch
                {
                    null => null,
                    Mapping.AsDestination when side == MemberSide.Destination => mapMemberAttr.Name,
                    Mapping.AsSource when side == MemberSide.Source => mapMemberAttr.Name,
                    _ => null
                };


#else
                if (mapMemberAttr.Mapping == null)
                {
                    return null;
                }
                
                if (mapMemberAttr.Mapping == Mapping.AsDestination && side == MemberSide.Destination)
                {
                    return mapMemberAttr.Name;
                }
                if (mapMemberAttr.Mapping == Mapping.AsSource && side == MemberSide.Source)
                {
                    return mapMemberAttr.Name;
                }
                
                return null;
#endif
            }));

            TypeAdapterConfig.GlobalSettings.Default.IgnoreMember((member, side) =>
            {
                var mapIgnore = member.GetCustomAttribute<MapIgnoreAttribute>();
                if (mapIgnore != null)
                {
                    if (mapIgnore.Mapping.HasValue)
                    {
                        if (mapIgnore.Mapping == Mapping.AsDestination && side == MemberSide.Destination)
                        {
                            return true;
                        }

                        if (mapIgnore.Mapping == Mapping.AsSource && side == MemberSide.Source)
                        {
                            return true;
                        }

                        return false;
                    }


                    return true;
                }

                return false;
            });
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