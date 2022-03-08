using System;
using System.Collections.Generic;
using System.Linq;
using ImmediateReflection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.DependencyInjection
{
    public static class ExportServiceScanner
    {
        public static List<Type> GetServices(Type type)
        {
            var exportServiceType = type.GetAllImmediateAttributes(true).OfType<ExportAttribute>().DefaultIfEmpty(new ExportAttribute { UseDefault = true, IncludeSelf = true })
                .SelectMany(x => x.GetExposedServiceTypes(type)).Distinct().ToList();
            return exportServiceType;
        }
    }
}