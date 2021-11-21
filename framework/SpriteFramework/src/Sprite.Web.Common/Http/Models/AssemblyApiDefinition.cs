using System;
using System.Collections.Generic;

namespace Sprite.Web.Http.Models
{
    public class AssemblyApiDefinition
    {
        public AssemblyApiDefinition()
        {
            Controllers = new List<ControllerDefinition>();
        }

        public AssemblyApiDefinition(string assemblyName, string path, string groupName) : this()
        {
            AssemblyName = assemblyName;
            Path = path;
            GroupName = groupName;
        }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; set; }

        public string Path { get; set; }

        public IList<ControllerDefinition> Controllers { get; set; }

        public string GroupName { get; set; }

        public ControllerDefinition AddController(string controllerName, Type controllerType)
        {
            return new(controllerName, controllerType);
        }
    }
}