using System;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.Tests;
using Xunit;

namespace Sprite.ObjectMapping.Mapster.Tests
{
    public class MapsterModuleBaseTests : SpriteIntegratedTest<MapsterTestModule>
    {
        private readonly IObjectMapper _objectMapper;

        public MapsterModuleBaseTests()
        {
            _objectMapper = ServiceProvider.GetRequiredService<IObjectMapper>();
        }


        [Fact]
        public void Should_Be_MapsterObjectMapper_Imp()
        {
            Assert.True(_objectMapper is MaptserObjectMapper);
        }

        [Fact]
        public void GlobalSettings_Should_Use_Scan_Config_Map_Source_To_Destination()
        {
            var booDto = _objectMapper.Map(new Boo()
            {
                Age = 1
            }, new BooDto());

            booDto.Age.ShouldBe(1);
            booDto.Id.ShouldBe(Guid.Empty);

            var id = Guid.NewGuid();
            var booDto2 = _objectMapper.Map<Boo, BooDto>(new Boo()
            {
                Id = id,
                Age = 2
            });

            booDto2.Id.ShouldBe(id);
            booDto2.Age.ShouldBe(2);
            
            
            //source to destination will be ignore map 
            _objectMapper.Map<BooDto,BooDto>(booDto2).Age.ShouldBe(default);

            var dateTime = DateTime.Now;
            var foo = new Foo
            {
                Id = id,
                Name = "foo1",
                Age = 1,
                Birthday = dateTime
            };

            //static func map
            var fooDto = foo.Adapt<Foo, FooDto>(); 
            
            fooDto.Id.ShouldBe(id);
            fooDto.Name.ShouldBeNull(); // this item is set to ignore
            fooDto.Age.ShouldBe(1);
            fooDto.Birthday.ShouldBe(dateTime);
        }

        

        
        [Fact]
        public void  Should_Map_Objects_With_Custom_Config()
        {
            var id = Guid.NewGuid();
            var dateTime = DateTime.Now;
            // var fooDto = _objectMapper.Map<Foo, FooDto>(new Foo
            // {
            //     Id = id,
            //     Name = "foo1",
            //     Age = 1,
            //     Birthday = dateTime
            // });
            //
            // fooDto.Id.ShouldBe(id);
            // fooDto.Name.ShouldBeNull(); // this item is set to ignore
            // fooDto.Age.ShouldBe(1);
            // fooDto.Birthday.ShouldBe(dateTime);

            var fooDto2 = _objectMapper.Map<Foo, FooDto2>(new Foo
            {
                Id = id,
                Name = "foo2",
                Age = 1,
                Birthday = dateTime
            });

            fooDto2.Id.ShouldBe(id);
            fooDto2.Name.ShouldBeNull(); // this item is set to [NotMap]
            fooDto2.Age.ShouldBe(1);
            fooDto2.Birthday.ShouldBe(default); // this item is set to  [AdaptIgnore(MemberSide.Destination)]
        }
    }
}