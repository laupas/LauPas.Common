using System;
using FluentAssertions;
using LauPas.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    internal class Complex
    {
        public string Key1 { get; set; }
        public string Key2 { get; set; }
    }

    [TestClass]
    public class ConfigServiceTest : BaseTest
    {
        [TestMethod]
        public void Get_String_NoValueInConfigFile_NoInjectedValue_NoValueInEnv_GetDefaultValue()
        {
            // Arrange
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get("non_existing_value", "default");

            //Assert
            value.Should().Be("default");
        }
        
        [TestMethod]
        public void Get_String_ValueInConfigFile_NoInjectedValue_NoValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("STRING_VALUE", string.Empty);
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("string_value");

            //Assert
            value.Should().Be("abcd");
        }

        [TestMethod]
        public void Get_String_ValueInConfigFile_InjectedValue_NoValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("STRING_VALUE", string.Empty);
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");
            Starter.Get.Resolve<IConfigService>().SetValue("string_value", "some other value");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("string_value");

            //Assert
            value.Should().Be("some other value");
        }

        [TestMethod]
        public void Get_String_ValueInConfigFile_NoInjectedValue_ValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("STRING_VALUE", "value from env");
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("string_value");

            //Assert
            value.Should().Be("value from env");
        }
        
        [TestMethod]
        public void Get_int_ValueInConfigFile_NoInjectedValue_NoValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("NUMBER_VALUE", string.Empty);
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<int>("number_value");

            //Assert
            value.Should().Be(1234);
        }
        
        [TestMethod]
        public void Get_int_ValueInConfigFile_InjectedValue_NoValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("NUMBER_VALUE", string.Empty);
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");
            Starter.Get.Resolve<IConfigService>().SetValue("number_value", 6789);

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<int>("number_value");

            //Assert
            value.Should().Be(6789);
        }
        [TestMethod]
        public void Get_int_ValueInConfigFile_NoInjectedValue_ValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("NUMBER_VALUE", "4567");
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<int>("number_value");

            //Assert
            value.Should().Be(4567);
        }

        [TestMethod]
        public void Get_bool_ValueInConfigFile_NoInjectedValue_NoValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("BOOL_VALUE", string.Empty);
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<bool>("bool_value");

            //Assert
            value.Should().Be(true);
        }

        [TestMethod]
        public void Get_bool_ValueInConfigFile_InjectedValue_NoValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("BOOL_VALUE", string.Empty);
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");
            Starter.Get.Resolve<IConfigService>().SetValue("bool_value", true);

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<bool>("bool_value");

            //Assert
            value.Should().Be(true);
        }
        
        [TestMethod]
        public void Get_bool_ValueInConfigFile_NoInjectedValue_ValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("BOOL_VALUE", "false");
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<bool>("bool_value");

            //Assert
            value.Should().Be(false);
        }

        [TestMethod]
        public void Get_InjectedValue_NoConfigFile_NoEnvValue_GetValue()
        {
            // Arrange
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetValue("key1", "Value1");
            
            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("key1");

            //Assert
            value.Should().Be("Value1");
        }
                
        [TestMethod]
        public void Get_Complex_ValueInConfigFile_NoInjectedValue_NoValueInEnv_GetValue()
        {
            // Arrange
            this.StartAllServices();
            Environment.SetEnvironmentVariable("COMPLEX_VALUE__KEY1", string.Empty);
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<Complex>("complex_value");

            //Assert
            value.Key1.Should().Be("value2 key1");
            value.Key2.Should().Be("value2 key2");
        }

        [TestMethod]
        public void Get_Complex_ValueInConfigFile_NoInjectedValue_ValueInEnv_GetValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("COMPLEX_VALUE__KEY1", "another key 1 value");
            this.StartAllServices();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<Complex>("complex_value");

            //Assert
            value.Key1.Should().Be("another key 1 value");
            value.Key2.Should().Be("value2 key2");
        }
    }
}