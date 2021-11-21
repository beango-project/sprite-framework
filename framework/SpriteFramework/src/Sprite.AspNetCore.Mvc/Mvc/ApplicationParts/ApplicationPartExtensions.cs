using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.ApplicationParts
{
    public static class ApplicationPartExtensions
    {
        public static void AddIfNotContains<T>(this IList<ApplicationPart> applicationParts)
        {
            AddIfNotContains(applicationParts, typeof(T).Assembly);
        }


        public static void AddIfNotContains(this IList<ApplicationPart> applicationParts, Assembly assembly)
        {
            if (applicationParts.Any(p => p is AssemblyPart assemblyPart && assemblyPart.Assembly == assembly))
            {
                return;
            }

            applicationParts.Add(new AssemblyPart(assembly));
        }
    }
}