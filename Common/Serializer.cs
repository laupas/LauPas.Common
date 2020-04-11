using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LauPas.Common
{
    /// <summary>
    /// Serialization Helper
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static T Deserialize<T>(this string data)
        {
            var logger = Starter.Get?.Resolve<ILoggerFactory>().CreateLogger(typeof(Serializer).Name);
            if (data.StartsWith("<?xml"))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));

                using(var reader = new StringReader(data))
                {
                    var result = (T)xmlSerializer.Deserialize(reader);
                    logger?.LogTrace($"Data to be serialized: XML => {result}");
                    return result;
                }
            }
            
            if (data.StartsWith("---"))
            {
                logger?.LogTrace($"Data to be serialized: YML");
                throw new NotImplementedException($"this kind of Data '{data}' can not be deserialized");
            }
            
            if (typeof(T) == typeof(string))
            {
                logger?.LogTrace($"Data to be serialized: String => {data}");
                return (T)(object)data;
            }
            else
            {
                logger?.LogTrace($"Data to be serialized: JSON");
                return JsonConvert.DeserializeObject<T>(data);
            }
        }
        
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serializationType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string Serialize<T>(this T data, SerializationType serializationType = SerializationType.Xml)
        {
            var logger = Starter.Get?.Resolve<ILoggerFactory>().CreateLogger(typeof(Serializer).Name);
            if (serializationType == SerializationType.Xml)
            {
                var xmlSerializer = new XmlSerializer(typeof(T));

                using(var stringWriter = new StringWriter())
                {
                    using(var writer = XmlWriter.Create(stringWriter))
                    {
                        xmlSerializer.Serialize(writer, data);
                        logger?.LogTrace($"Data to be deserialized: XML => {stringWriter}");
                        return stringWriter.ToString();
                    }
                }   
            }
            else
            {
                throw new NotImplementedException($"this kind of Data '{serializationType}' can not be deserialized");
            }
        }

    }
}