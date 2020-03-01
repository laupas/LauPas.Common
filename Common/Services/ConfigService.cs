using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LauPas.Common.Services
{
    [Singleton]
    internal class ConfigService : IConfigService
    {
        private string configFile;
        private ILogger logger;

        public ConfigService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var valueFromEnv = Environment.GetEnvironmentVariable(key.ToUpperInvariant());
            if (!string.IsNullOrEmpty(valueFromEnv))
            {
                this.logger.LogInformation($"Found key {key} in Env variable");
                return deserializer.Deserialize<T>(valueFromEnv);
            }
            
            if (string.IsNullOrEmpty(this.configFile))
            {
                throw new Exception($"ConfigFile not set. Call SetConfigFile before.");
            }

            if (!File.Exists(this.configFile))
            {
                throw new Exception($"ConfigFile {Path.GetFullPath(this.configFile)} not found.");
            }

            var input = File.ReadAllText(this.configFile);
            
            var value = deserializer.Deserialize<Dictionary<string, object>>(input);
            if (value == null)
            {
                if (defaultValue != null)
                {
                    return defaultValue;
                }

                throw new Exception($"Config file looks empty.");
            }

            var keyResult = value.Keys.SingleOrDefault(k => k.ToUpperInvariant() == key.ToUpperInvariant());

            if (keyResult == null)
            {
                if (defaultValue != null)
                {
                    return defaultValue;
                }

                throw new Exception($"Key {key} not found in config file.");
            }
            this.logger.LogInformation($"Found key {key} in config file");

            return (T) value[keyResult];
        }

        public void SetConfigFile(string configFileToBeUsed)
        {
            this.configFile = configFileToBeUsed;
        }
    }
}