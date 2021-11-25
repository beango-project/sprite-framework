using Mapster;

namespace Sprite.ObjectMapping.Mapster.Tests
{
    public class TestMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Foo, FooDto>().Ignore(x => x.Name);

            config.ForType<Foo, FooDto2>();
            
            config.Compile();
        }
    }
}