using System;
using Mapster;
using Sprite.ObjectMapping.Mapster.Attributes;

namespace Sprite.ObjectMapping.Mapster.Tests
{
    public class Foo
    {
        public Guid Id { get; set; }

        public int Age { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }
    }

    public class FooDto
    {
        public Guid Id { get; set; }

        public int Age { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }
    }

    public class FooDto2
    {
        public Guid Id { get; set; }

        public int Age { get; set; }

        [NotMap]
        public string Name { get; set; }

        [AdaptIgnore(MemberSide.Destination)]
        public DateTime Birthday { get; set; }
    }

    public class Boo
    {
        public Guid Id { get; set; }


        public int Age { get; set; }
    }

    public class BooDto
    {
        public Guid Id { get; set; }

        [AdaptIgnore(MemberSide.Source)]
        public int Age { get; set; }
    }
}