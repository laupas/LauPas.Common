using System;
using FluentAssertions;
using LauPas.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    [TestClass]
    public class ConfigServiceTest : BaseTest
    {
        [TestMethod]
        public void Get_ValueInConfigFile_NoValueInEnv_GetValue()
        {
            // Arrange
            this.StartAllServices<IConfigService>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("value1");

            //Assert
            value.Should().Be("abcd");
        }
        
        [TestMethod]
        public void Get_ValueInConfigFile_NoValueInEnv_GetValueWithUpperCaseKey()
        {
            // Arrange
            Environment.SetEnvironmentVariable("VALUE1", string.Empty);
            this.StartAllServices<IConfigService>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("VALUE1");

            //Assert
            value.Should().Be("abcd");
        }

        [TestMethod]
        public void Get_NoValueInConfigFile_NoValueInEnv_GetDefaultValue()
        {
            // Arrange
            Environment.SetEnvironmentVariable("OtherValue", string.Empty);
            this.StartAllServices<IConfigService>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get("OtherValue", "DefaultValue");

            //Assert
            value.Should().Be("DefaultValue");
        }

        [TestMethod]
        public void Get_ValueInConfigFile_ValueInEnv_GetValue()
        {
            // Arrange
            this.Arguments.Add("--value1=newValue");
            this.StartAllServices<IConfigService>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("value1");

            //Assert
            value.Should().Be("newValue");
        }
        
        [TestMethod]
        public void Get_NoConfigFile_ValueInEnv_GetValue()
        {
            // Arrange
            this.Arguments.Add("--value1=newValue");
            this.StartAllServices<IConfigService>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile("config.yml");

            // Act
            var value = Starter.Get.Resolve<IConfigService>().Get<string>("value1");

            //Assert
            value.Should().Be("newValue");
        }
    }
}