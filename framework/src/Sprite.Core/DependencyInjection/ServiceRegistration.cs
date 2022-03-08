using System;
using JetBrains.Annotations;

namespace Sprite.DependencyInjection
{
    public record ServiceRegistration
    {
        [CanBeNull]
        public Type? ConcreteType;

        [CanBeNull]
        public string? Named { get; }

        [CanBeNull]
        public object? Key { get; }

        public Type ServiceType;
        
        public ServiceRegistrationPattern Pattern { get; }
    }
}