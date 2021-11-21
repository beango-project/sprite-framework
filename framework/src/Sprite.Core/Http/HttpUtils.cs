using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Sprite.Http
{
    public static class HttpUtils
    {
        public static Dictionary<string, string[]> ConventionalHttpMethod { get; set; } = new()
        {
            {"GET", new[] {"GetList", "GetAll", "Get"}},
            {"PUT", new[] {"Put", "Update"}},
            {"DELETE", new[] {"Delete", "Remove"}},
            {"POST", new[] {"Create", "Add", "Insert", "Post"}},
            {"PATCH", new[] {"Patch"}}
        };

        public static string[] CommonPostfixes { get; set; } = { "AppService", "ApplicationService", "Service" };
        public static string ConvertHttpMethod(string methodName, string defaultMethod = "POST")
        {
            foreach (var conventionalPrefix in ConventionalHttpMethod
                .Where(conventionalPrefix => conventionalPrefix.Value.Any(prefix => methodName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))))
            {
                return conventionalPrefix.Key;
            }

            return defaultMethod;
        }

        public static string RemoveHttpMethodPrefix([NotNull] string methodName, [NotNull] string httpMethod)
        {
            Check.NotNull(methodName, nameof(methodName));
            Check.NotNull(httpMethod, nameof(httpMethod));
            string[] prefixes;

            ConventionalHttpMethod.TryGetValue(httpMethod, out prefixes);

            if (prefixes == null || prefixes.Length <= 0)
            {
                return methodName;
            }

            return methodName.RemovePrefix(prefixes);
        }
    }
}