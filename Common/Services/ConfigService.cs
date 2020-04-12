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
        private readonly ILogger logger;
        private readonly IDictionary<string, object> values = new Dictionary<string, object>();

        public ConfigService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
        }

        public T Get<T>(string keyName, T defaultValue = default(T))
        {
            this.logger.LogTrace($"Search for {keyName} in ConfigService with default: {defaultValue != null}");
            var key = keyName.ToUpperInvariant();
            T result;
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var resultType = typeof(T);
            if (resultType.IsPrimitive || resultType == typeof(Decimal) || resultType == typeof(String)  || resultType == typeof(DateTime))
            {
                result = (T)this.HandleSimpleType(key, resultType, deserializer);
            }
            else
            { 
                result = (T)this.HandleComplexType<T>(key, resultType, deserializer);
            }

            if (result == null && defaultValue != null)
            {
                return defaultValue;
            }

            if (result != null)
            {
                return result;
            }
            
            throw new KeyNotFoundException($"Key {keyName} not found in ConfigService");
        }

        private object HandleSimpleType(string key, Type type, IDeserializer deserializer)
        {
            this.logger.LogTrace($"{key} {type.Name} is a simple object");
            if (Environment.GetEnvironmentVariables().Contains(key))
            {
                this.logger.LogTrace($"Found SimpleType {key} in EnvironmentVariables");
                return deserializer.Deserialize(Environment.GetEnvironmentVariable(key), type);
            }

            if (this.values.ContainsKey(key))
            {
                this.logger.LogTrace($"Found SimpleType {key} in ConfigService");
                return deserializer.Deserialize(this.values[key].ToString(), type);
            }

            return null;
        }

        private object HandleComplexType<T>(string key, Type type, IDeserializer deserializer)
        {
            this.logger.LogTrace($"{key} {type.Name} is a complex object");

            var nonDefault = false;
            var result = Activator.CreateInstance<T>();
            foreach (var property in type.GetProperties())
            {
                var propKeyName = $"{key.ToUpperInvariant()}__{property.Name.ToUpperInvariant()}";
                this.logger.LogTrace($"Search for property {property.Name} with key {propKeyName}");
                var tempKeyName = Environment.GetEnvironmentVariables().Keys.Cast<string>().FindEntryWithUndefinedCasesInList(propKeyName);

                if (!string.IsNullOrEmpty(tempKeyName))
                {
                    this.logger.LogTrace($"Found ComplexType Property {property.Name} in EnvironmentVariables");
                    var valueFromEnv = deserializer.Deserialize(Environment.GetEnvironmentVariable(tempKeyName),
                        property.PropertyType);
                    property.SetValue(result, valueFromEnv);
                    nonDefault = true;
                }
                else
                {
                    if (!this.values.ContainsKey(key))
                    {
                        continue;
                    }
                    
                    var temp = this.values[key];
                    if (!(temp is YamlMappingNode node))
                    {
                        continue;
                    }
                    
                    var tempKeyNameToSearch = node.Children.Select(n => n.Key.ToString()).FindEntryWithUndefinedCasesInList(property.Name);
                    if (!string.IsNullOrEmpty(tempKeyNameToSearch))
                    {
                        this.logger.LogTrace($"Found ComplexType Property {property.Name} in ConfigService");
                        var valueFromValues =
                            deserializer.Deserialize(node.Children[tempKeyNameToSearch].ToString(), property.PropertyType);
                        property.SetValue(result, valueFromValues);
                        nonDefault = true;
                    }

                }
            }

            if (nonDefault)
            {
                return result;
            }
            
            return null;
        }

        public void SetConfigFile(string configFileToBeUsed)
        {
            if (string.IsNullOrEmpty(configFileToBeUsed))
            {
                throw new Exception($"ConfigFile not set. Call SetConfigFile before.");
            }

            if (!File.Exists(configFileToBeUsed))
            {
                this.logger.LogWarning($"File {Path.GetFullPath(configFileToBeUsed)} not found.");
                return;
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