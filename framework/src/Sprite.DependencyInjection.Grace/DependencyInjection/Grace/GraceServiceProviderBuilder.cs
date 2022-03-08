using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AspectCore.Extensions.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;
using ImmediateReflection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.DependencyInjection.Grace
{
    public static class GraceServiceProviderBuilder
    {
        public static IInjectionScope Build()
        {
            var container = new DependencyInjectionContainer();
            // container.Configure(c => c.ImportMembers(info => info.IsDefinedAttribute(typeof(AutowiredAttribute), true)));
            container.Add(new MyConfigurationModule());

            return container;
        }
    }

    public class MyConfigurationModule : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock registrationBlock)
        {
            registrationBlock.AddMemberInjectionSelector(new MyMemberInjectionSelector());
        }
    }

    public class MyMemberInjectionSelector : IMemberInjectionSelector
    {
        public IEnumerable<MemberInjectionInfo> GetPropertiesAndFields(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            foreach (var declaredMember in type.GetTypeInfo().DeclaredMembers)
            {
                Type importType = null;
                var propertyInfo = declaredMember as PropertyInfo;

                if (propertyInfo != null)
                {
                    if (propertyInfo.CanWrite)
                    {
                        importType = propertyInfo.PropertyType;
                    }
                }
                else if (declaredMember is FieldInfo fieldInfo)
                {
                    importType = fieldInfo.FieldType;
                }

                if (importType == null)
                {
                    continue;
                }

                var attr = declaredMember.GetAttributeWithDefined<ImportAttribute>();

                if (attr != null)
                {
                    // Try to set the default value
                    object localValue = null;
                    if (attr.Type != null)
                    {
                        injectionScope.TryLocate(attr.Type, out localValue);
                    }


                    yield return new MemberInjectionInfo()
                    {
                        MemberInfo = declaredMember,
                        IsRequired = attr.IsRequired,
                        DefaultValue = localValue,//Given value, but doesn't work
                    };
                }
            }
        }

        public IEnumerable<MethodInjectionInfo> GetMethods(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            yield break;
        }
    }

    class ImportAttribute : Attribute
    {
        public Type Type { get; set; }

        public bool IsRequired { get; set; }
    }
}