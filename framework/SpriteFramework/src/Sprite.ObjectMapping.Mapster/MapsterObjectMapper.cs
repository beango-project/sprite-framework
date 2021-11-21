namespace Sprite.ObjectMapping.Mapster
{
    public class MapsterObjectMapper : IObjectMapper
    {
        private readonly IMapperAccessor _accessor;

        public MapsterObjectMapper(IMapperAccessor accessor)
        {
            _accessor = accessor;
        }

        public TDestination Map<TDestination>(object source)
        {
            return _accessor.Mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _accessor.Mapper.Map(source, destination);
        }
    }
}