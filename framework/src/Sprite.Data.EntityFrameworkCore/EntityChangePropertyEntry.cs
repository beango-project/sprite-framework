using System;
using System.Collections.Immutable;
using System.Reflection;
using AspectCore.Extensions.Reflection;
using ImTools;

namespace Sprite.Data.EntityFrameworkCore
{
    //TODO:管道化，使其可以执行自定义逻辑
    public record EntityChangePropertyEntry
    {
        private PropertyReflector _property;

        public Type AnnotationType { get; }

        public EntityChangePropertyEntry(PropertyInfo propertyInfo, Type annotationType)
        {
            _property = propertyInfo.GetReflector();
            AnnotationType = annotationType;
        }

        public bool IsCandidate(Type annotationType) => AnnotationType == annotationType;


        public PropertyInfo GetProperty() => _property.GetPropertyInfo();

        public void SetValue(object instance, object? value)
        {
            _property.SetValue(instance, value);
        }
    }
}