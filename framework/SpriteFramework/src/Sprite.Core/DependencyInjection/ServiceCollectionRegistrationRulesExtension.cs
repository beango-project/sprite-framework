using System;
using System.Linq;
using System.Reflection;
using Sprite.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionRegistrationRulesExtension
    {
        public static IServiceCollection AddRegistrationRules(this IServiceCollection services, IRegistrationRules rules)
        {
            GetOrCreateEffectiveRegistrationRules(services).AddRules(rules);
            return services;
        }

        public static EffectiveRegistrationRules GetEffectiveRegistrationRules(this IServiceCollection services)
        {
            return GetOrCreateEffectiveRegistrationRules(services);
        }

        private static EffectiveRegistrationRules GetOrCreateEffectiveRegistrationRules(IServiceCollection services)
        {
            var effectiveRegistrationRules = services.GetSwapSpace().TryGet<EffectiveRegistrationRules>();
            if (effectiveRegistrationRules == null)
            {
                effectiveRegistrationRules = new EffectiveRegistrationRules();
                effectiveRegistrationRules.AddRules(new DefaultRegistrationRules());
                services.AddInSwapSpace<EffectiveRegistrationRules>(effectiveRegistrationRules);
            }

            return effectiveRegistrationRules;
        }

        public static IServiceCollection AddFromAssemblyOf<T>(this IServiceCollection services)
        {
            return services.AddFromAssemblyOf(typeof(T).GetTypeInfo().Assembly);
        }

        public static IServiceCollection AddFromAssemblyOf(this IServiceCollection services, Assembly assembly)
        {
            var effectiveRegistrationRules = services.GetEffectiveRegistrationRules();
            
            foreach (var rules in effectiveRegistrationRules.RulesCollection)
            {
                rules.AddFromAssemblyOf(services, assembly);
            }

            return services;
        }

        public static IServiceCollection AddFromTypeOf(this IServiceCollection services, Type type)
        {
            var effectiveRegistrationRules = services.GetEffectiveRegistrationRules();
            foreach (var rules in effectiveRegistrationRules.RulesCollection)
            {
                rules.AddFromTypeOf(services, type);
            }

            return services;
        }
    }
}