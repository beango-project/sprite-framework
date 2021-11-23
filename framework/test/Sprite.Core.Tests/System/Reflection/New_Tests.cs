using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Sprite.Core.Tests.System.Reflection
{
    public class New_Tests
    {
        class MyClass
        {
            public Guid Id => Guid.NewGuid();
        }


        [Fact]
        public void New_Object()
        {
            var instance = New<MyClass>.Instance.Invoke();
            Assert.NotNull(instance);
            instance.Id.ShouldNotBe(Guid.Empty);
        }
    }
}