using System;
using System.Linq;
using CommonTests.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.DependencyInjection;
using Sprite.DependencyInjection.Attributes;
using Xunit;

namespace Sprite.Core.Tests.DependencyInjection
{
    //NOTE: Clear Swap Space after each test to facilitate subsequent tests
    public class ConventionalDependencyInjection_Tests
    {
        private readonly IServiceCollection _services;

        public ConventionalDependencyInjection_Tests()
        {
            _services = new ServiceCollection();
            _services.TryAddSwapSpace();
        }

        [Fact]
        public void AddSwapSpace_Test()
        {
            _services.GetSwapSpace().ShouldNotBeNull();

            var serviceDescriptor1 = _services.FirstOrDefault(x => x.ServiceType == typeof(ISwapSpace));
            var serviceDescriptor2 = _services.FirstOrDefault(x => x.ServiceType == typeof(SwapSpace));

            serviceDescriptor1.ImplementationInstance.ShouldNotBeNull();
            serviceDescriptor1.ImplementationInstance.ShouldBeOfType<SwapSpace>();

            serviceDescriptor2.ImplementationInstance.ShouldNotBeNull();
            serviceDescriptor2.ImplementationInstance.ShouldBeOfType<SwapSpace>();
        }


        [Fact]
        public void AddInSwapSpace_Get_And_Set_Test()
        {
            //add and tryGet&get,after set test
            var swapSpace = _services.GetSwapSpace();
            _services.AddInSwapSpace<SingletonClassInject1>();

            var singletonClassInject1 = swapSpace.Get<SingletonClassInject1>();
            swapSpace.TryGet(out singletonClassInject1).ShouldBe(true);
            singletonClassInject1.ShouldBeNull();


            var tmp = singletonClassInject1 = new SingletonClassInject1(); //after set value

            singletonClassInject1.ShouldNotBeNull();
            singletonClassInject1.ShouldBe(tmp);

            //add and set test
            var singletonClassInject2 = new SingletonClassInject2();
            swapSpace.Add<SingletonClassInject2>(singletonClassInject2); // add set
            tmp = swapSpace.Get<SingletonClassInject2>();
            tmp.ShouldBe(singletonClassInject2);

            //GetSet test
            swapSpace.Clear();

            var instance1 = new SingletonClassInject1();
            _services.AddInSwapSpace<SingletonClassInject1>(instance1);

            var instance2 = new SingletonClassInject1();
            var fromSwapSpaceInstance = swapSpace.GetSet<SingletonClassInject1>(instance2); //get set
            instance1.ShouldBe(fromSwapSpaceInstance);
            instance2.ShouldBe(swapSpace.Get<SingletonClassInject1>());

            swapSpace.Count().ShouldBe(1);
            swapSpace.Clear();
        }

        [Fact]
        public void SwapSpace_TryAdd_Test()
        {
            var swapSpace = _services.GetSwapSpace();
            var instance1 = new SingletonClassInject1();
            swapSpace.Add<SingletonClassInject1>();
            swapSpace.TryAdd<SingletonClassInject1>(instance1);
            
            swapSpace.Get<SingletonClassInject1>().ShouldBeNull();
            swapSpace.Clear();
        }


        [Fact]
        public void SwapSpace_Count_IsEmpty_Test()
        {
            var swapSpace = _services.GetSwapSpace();
            swapSpace.Clear();

            swapSpace.IsEmpty.ShouldBe(true);
            swapSpace.Count().ShouldBe(0);

            //add 
            swapSpace.Add<SingletonClassInject1>();
            swapSpace.Add<SingletonClassInject2>(new SingletonClassInject2());

            swapSpace.IsEmpty.ShouldBe(false);
            swapSpace.Count().ShouldBe(2);

            swapSpace.Clear();
        }

        [Fact]
        public void SwapSpaceClear_Test()
        {
            var swapSpace = _services.GetSwapSpace();
            swapSpace.Add<SingletonClassInject1>(new SingletonClassInject1());

            swapSpace.Count().ShouldBe(1);
            swapSpace.IsEmpty.ShouldBe(false);

            swapSpace.Clear();
        }

        [Fact]
        public void Should_Inject_Transient()
        {
            _services.AddFromTypeOf(typeof(TransientClassInject));
            _services.ShouldBeTransient(typeof(TransientClassInject));
        }

        [Fact]
        public void Should_Inject_Scope()
        {
            _services.AddFromTypeOf(typeof(ScopedClassInject));
            _services.ShouldBeScope(typeof(ScopedClassInject));
        }

        [Fact]
        public void Should_Inject_Singleton()
        {
            _services.AddFromTypeOf(typeof(SingletonClassInject1));
            _services.ShouldBeSingleton(typeof(SingletonClassInject1));
        }


        [Fact]
        public void Should_Inject_Transient_With_Attribute()
        {
            _services.AddFromTypeOf(typeof(TransientClassWithAttributeInject));
            _services.ShouldBeTransient(typeof(TransientClassWithAttributeInject));
        }

        [Fact]
        public void Should_Inject_Scope_With_Attribute()
        {
            _services.AddFromTypeOf(typeof(ScopeClassWithAttributeInject));
            _services.ShouldBeScope(typeof(ScopeClassWithAttributeInject));
        }

        [Fact]
        public void Should_Inject_Singleton_With_Attribute()
        {
            _services.AddFromTypeOf(typeof(SingletonClassWithAttributeInject));
            _services.ShouldBeSingleton(typeof(SingletonClassWithAttributeInject));
        }

        public class TransientClassInject : ITransientInjection
        {
        }

        public class SingletonClassInject1 : ISingletonInjection
        {
            public Guid Id => Guid.NewGuid();
        }

        public class SingletonClassInject2 : SingletonClassInject1
        {
        }


        public class ScopedClassInject : IScopeInjection
        {
        }

        [Component(ServiceLifetime.Transient)]
        public class TransientClassWithAttributeInject
        {
        }

        [Component(ServiceLifetime.Scoped)]
        public class ScopeClassWithAttributeInject
        {
        }

        [Component(ServiceLifetime.Singleton)]
        public class SingletonClassWithAttributeInject
        {
        }
    }
}