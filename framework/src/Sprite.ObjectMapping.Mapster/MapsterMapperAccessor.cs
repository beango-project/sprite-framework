using MapsterMapper;

namespace Sprite.ObjectMapping.Mapster
{
    internal class MapsterMapperAccessor : IMapperAccessor
    {
        public IMapper Mapper { get; set; }
    }
}