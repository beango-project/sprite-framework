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
            container.Add(new SpriteGraceConfigurationModule());

            return container;
        }
    }

    public class SpriteGraceConfigurationModule : IConfigurationModule
    {
        private readonly Assembly _exportAssembly;

        public SpriteGraceConfigurationModule(Assembly exportAssembly = null)
        {
            _exportAssembly = exportAssembly ?? null;
        }

        public void Configure(IExportRegistrationBlock registrationBlock)
        {
            if (_exportAssembly != null)
            {
                registrationBlock.ExportAssembly(_exportAssembly).ExportAttributedTypes().Where(TypesThat.AreInTheSameNamespaceAs<AutowiredAttribute>());
            }

            registrationBlock.AddMemberInjectionSelector(new SpriteMemberInjectionSelector());
        }
    }

    public class SpriteMemberInjectionSelector : IMemberInjectionSelector
    {
        public IEnumerable<MemberInjectionInfo> GetPropertiesAndFields(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            var memberInfos = new List<MemberInfo>();
            var currentType = type;

            while (currentType != null && currentType.Name != nameof(Object))
            {
                memberInfos.AddRange(currentType.GetTypeInfo().DeclaredMembers);
                currentType = currentType.BaseType;
            }
            
            foreach (var declaredMember in memberInfos.Where(m => m is PropertyInfo property && property.CanWrite || m is FieldInfo))
            {
                var attr = declaredMember.GetAttributeWithDefined<AutowiredAttribute>();

                if (attr != null)
                {
                    object value = null;
                    object Key = null;
                    if (attr.Type != null)
                    {
                        if (attr.Key != null)
                        {
                            Key = attr.Key;
                        }
                        else
                        {
                            Key = attr.Type;
                        }

                        // if (injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(attr.Type))
                        // {
                        //     
                        // }
                        injectionScope.TryLocate(attr.Type, out value);
                    }

                    yield return new MemberInjectionInfo()
                    {
                        MemberInfo = declaredMember,
                        IsRequired = attr.IsRequired,
                        DefaultValue = value,
                        LocateKey = Key
                    };
                }
            }
        }

        public IEnumerable<MethodInjectionInfo> GetMethods(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            yield break;
        }
    }
}