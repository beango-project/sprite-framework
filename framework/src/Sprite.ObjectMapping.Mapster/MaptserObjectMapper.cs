using System.Threading.Tasks;
using Mapster;
using MapsterMapper;

namespace Sprite.ObjectMapping.Mapster
{
    public class MaptserObjectMapper : IObjectMapper
    {
        private readonly IMapper _mapper;

        public MaptserObjectMapper(IMapper mapper)
        {
            _mapper = mapper;
        }


        public TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }

        public async Task<TDestination> MapAsync<TDestination>(object source)
        {
            return await _mapper.From(source).AdaptToTypeAsync<TDestination>();
        }

        public async Task<TDestination> MapAsync<TSource, TDestination>(TSource source)
        {
            return await _mapper.From(source).AdaptToTypeAsync<TDestination>();
        }

        public async Task<TDestination> MapAsync<TSource, TDestination>(TSource source, TDestination destination)
        {
            return await _mapper.From(source).AdaptToAsync(destination);
        }

        #region Deprecated

        // private readonly IMapperAccessor _accessor;
        //
        // public MaptserObjectMapper(IMapperAccessor accessor)
        // {
        //     _accessor = accessor;
        // }
        //
        // public TDestination Map<TDestination>(object source)
        // {
        //     // return source.Adapt<TDestination>();
        //     return _accessor.Mapper.Map<TDestination>(source);
        // }
        //
        // public TDestination Map<TSource, TDestination>(TSource source)
        // {
        //     // return source.Adapt<TSource, TDestination>();
        //     return _accessor.Mapper.Map<TSource, TDestination>(source);
        // }
        //
        // public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        // {
        //     // return source.Adapt(destination);
        //     return _accessor.Mapper.Map(source, destination);
        // }

        #endregion
    }
}