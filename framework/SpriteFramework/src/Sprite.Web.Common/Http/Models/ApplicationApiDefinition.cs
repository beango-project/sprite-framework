using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sprite.Web.Http.Models
{
    /// <summary>
    /// 访问描述模型
    /// </summary>
    public class ApplicationApiDefinition
    {
        public string Path { get; set; }

        public IList<AssemblyApiDefinition> AssemblyApiDefinitions { get; set; }

        public AssemblyApiDefinition AddAssembly(Assembly assembly, string path, string groupName = "Default")
        {
            var assemblyApiDefinition = new AssemblyApiDefinition
            {
                AssemblyName = assembly.FullName,
                Path = path,
                GroupName = groupName
            };

            if (AssemblyApiDefinitions.AddIfNotContains(assemblyApiDefinition))
            {
                return assemblyApiDefinition;
            }

            if (AssemblyApiDefinitions.Any(x => x.AssemblyName == assembly.FullName))
            {
                throw new Exception($"There are duplicate assembly{assembly.FullName}");
            }

            return null;
        }

        public AssemblyApiDefinition GetOrAddAssembly(Assembly assembly, string path, string groupName = "Default")
        {
            if (AssemblyApiDefinitions.Any(x => x.AssemblyName == assembly.FullName))
            {
                return AssemblyApiDefinitions.Single(x => x.AssemblyName == assembly.FullName);
            }

            var assemblyApiDefinition = new AssemblyApiDefinition
            {
                AssemblyName = assembly.FullName,
                Path = path,
                GroupName = groupName
            };
            AssemblyApiDefinitions.Add(assemblyApiDefinition);
            return assemblyApiDefinition;
        }
    }
}