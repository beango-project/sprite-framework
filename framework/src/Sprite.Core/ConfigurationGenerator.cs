using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Sprite
{
    public static class ConfigurationGenerator
    {
        public static IConfigurationRoot Create(SpriteConfigurationOptions options = null, Action<IConfigurationBuilder> configBuilderOptions = null)
        {
            options ??= new SpriteConfigurationOptions();

            if (options.BasePath.IsNullOrEmpty())
            {
                options.BasePath = Directory.GetCurrentDirectory();
            }

            var config = new ConfigurationBuilder();
            config.SetBasePath(options.BasePath);
            var reloadOnChange = true;
            config.AddJsonFile("appsettings.json", true, reloadOnChange).AddJsonFile("appsettings." + options.Environment + ".json", true, reloadOnChange);
            if (options.IsDevelopment())
            {
                if (!options.UserSecretsId.IsNullOrEmpty())
                {
                    config.AddUserSecrets(options.UserSecretsId);
                }

                if (options.UserSecretsAssembly != null)
                {
                    config.AddUserSecrets(options.UserSecretsAssembly, true);
                }
            }

            config.AddEnvironmentVariables(options.EnvironmentVariablesPrefix);

            if (options.CommandLineArgs != null)
            {
                config.AddCommandLine(options.CommandLineArgs);
            }

            configBuilderOptions?.Invoke(config);
            return config.Build();
        }
    }
}