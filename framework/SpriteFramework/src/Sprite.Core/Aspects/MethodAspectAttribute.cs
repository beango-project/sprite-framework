using System;
using AspectInjector.Broker;

namespace Sprite.Aspects
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    [Injection(typeof(MethodAspect), Inherited = true)]
    public abstract class MethodAspectAttribute : Attribute
    {
        public  abstract Type  InjectionType { get; }
    }
}