using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LauPas.Common.Services
{
    [Singleton]
    internal class ConfigService : IConfigService
    {
        private ILogger logger;
        private IDictionary<string, object> values = new Dictionary<string, object>();

        public ConfigService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
        }

        public T Get<T>(string keyName, T defaultValue = default(T))
        {
            var key = keyName.ToUpperInvariant();
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var resultType = typeof(T);
            if (resultType.IsPrimitive || resultType == typeof(Decimal) || resultType == typeof(String)  || resultType == typeof(DateTime))
            {
                return (T)this.HandleSimpleType(key, resultType, deserializer);
            }

            if (this.values.ContainsKey(key))
            {
                var temp = this.values[key];
                if (temp is YamlMappingNode node)
                {
                    var result = Activator.CreateInstance<T>();
                    foreach (var yamlNode in node.Children)
                    {
                        var property = resultType.GetProperties().SingleOrDefault(p =>
                            p.Name.ToUpperInvariant() == yamlNode.Key.ToString().Replace("_",string.Empty).ToUpperInvariant());
                        if (property != null)
                        {
                            var propKeyName = $"{key}__{property.Name.ToUpperInvariant()}";
                            if (Environment.GetEnvironmentVariables().Contains(propKeyName))
                            {
                                this.logger.LogTrace($"Found {key}__{property.Name} in EnvironmentVariables");
                                var valueFromEnv = deserializer.Deserialize(Environment.GetEnvironmentVariable(propKeyName), property.PropertyType);
                                property.SetValue(result, valueFromEnv);
                            }
                            else
                            {
                                var valueFromValues = deserializer.Deserialize(yamlNode.Value.ToString(), property.PropertyType);
                                property.SetValue(result, valueFromValues);
                                
                            }
                        }
                    }

                    return result;
                }
                else if (temp is T)
                {
                    return (T) temp;
                } 
            }

            if (defaultValue != null)
            {
                return defaultValue;
            }
            
            throw new KeyNotFoundException($"Key {keyName} not found in ConfigService");
        }

        private object HandleSimpleType(string key, Type type, IDeserializer deserializer)
        {
            this.logger.LogTrace($"{key} {type.Name} is a simple object");
            if (Environment.GetEnvironmentVariables().Contains(key))
            {
                this.logger.LogTrace($"Found {key} in EnvironmentVariables");
                return deserializer.Deserialize(Environment.GetEnvironmentVariable(key), type);
            }

            if (this.values.ContainsKey(key))
            {
                this.logger.LogTrace($"Found {key} in ConfigService");
                return deserializer.Deserialize(this.values[key].ToString(), type);
            }
            
            throw new KeyNotFoundException($"Key {key} not found in ConfigService");
        }

        public void SetConfigFile(string configFileToBeUsed)
        {
            if (string.IsNullOrEmpty(configFileToBeUsed))
            {
                throw new Exception($"ConfigFile not set. Call SetConfigFile before.");
            }

            if (!File.Exists(configFileToBeUsed))
            {
                throw new Exception($"ConfigFile {Path.GetFullPath(configFileToBeUsed)} not found.");
            }
            
            var input = File.ReadAllText(configFileToBeUsed);
            
            var yaml = new YamlStream();
            yaml.Load(new StringReader(input));
            
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            foreach (var entry in mapping.Children)
            {
                this.values.Add(entry.Key.ToString().ToUpperInvariant(), entry.Value);
            }
        }

        public void SetValue<T>(string key, T value)
        {
            if (this.values.ContainsKey(key.ToUpperInvariant()))
            {
                this.values[key.ToUpperInvariant()] = value;
            }
            else
            {
                this.values.Add(key.ToUpperInvariant(), value);
            }
        }
    }
}