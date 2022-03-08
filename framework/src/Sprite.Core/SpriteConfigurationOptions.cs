using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Sprite
{
    public class SpriteConfigurationOptions
    {
        public Assembly UserSecretsAssembly { get; set; }

        public string UserSecretsId { get; set; }

        public string FileName { get; set; } = "appsettings";

        /// <summary>
        /// <see cref="Environments"/>
        ///  </summary>
        public string Environment { get; set; }

        public string BasePath { get; set; }

        public string EnvironmentVariablesPrefix { get; set; }

        public string[] CommandLineArgs { get; set; }

        public bool IsDevelopment() => Environment == Environments.Development;
    }
}