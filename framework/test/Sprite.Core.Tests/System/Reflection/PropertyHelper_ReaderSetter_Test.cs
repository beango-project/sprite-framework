using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Sprite.Core.Tests.System.Reflection
{
    public class PropertyHelper_ReaderSetter_Test
    {
        class MyClass
        {
            public string Name { get; set; }
            public DateTime Time { get; protected set; }

            public int Age { get; set; }
        }

        [Fact]
        public void PropertySet_SetProperty()
        {
            var myClass = new MyClass();
            var dateTime = DateTime.Now;

            new PropertySetter<int>(myClass, 18, myClass.GetType().GetProperty("Age"));
            new PropertySetter<DateTime>(myClass, dateTime, myClass.GetType().GetProperty("Time"));

            myClass.Age.ShouldBe(18);
            myClass.Time.ShouldBe(dateTime);
        }


        [Fact]
        public void PropertyReaderSetter_ReadAndSetProperty()
        {
            var myClass = new MyClass();
            
            var propertyReaderSetter1 = new PropertyReaderSetter<string>(myClass, myClass.GetType().GetProperty("Name"));
            
            propertyReaderSetter1.Value.ShouldBe(null);
            propertyReaderSetter1.Value = "hi";
            propertyReaderSetter1.Value.ShouldBe("hi");
        }
    }
}