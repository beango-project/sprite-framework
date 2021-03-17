using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sprite.Reflection
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// 获取解决方案下的程序集
        /// </summary>
        /// <returns>当前解决方案下面的所有程序集</returns>
        public static Assembly[] GetSolutionAssemblies()
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
            return assemblies.ToArray();
        }
    }
}