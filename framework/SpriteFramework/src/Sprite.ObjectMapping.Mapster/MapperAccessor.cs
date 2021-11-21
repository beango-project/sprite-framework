using MapsterMapper;

namespace Sprite.ObjectMapping.Mapster
{
    internal class MapperAccessor : IMapperAccessor
    {
        public IMapper Mapper { get; set; }
    }
}