using MapsterMapper;

namespace Sprite.ObjectMapping.Mapster
{
    public interface IMapperAccessor
    {
        IMapper Mapper { get; set; }
    }
}