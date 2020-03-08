using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LauPas.Common;
using Microsoft.Extensions.Logging;

namespace Common.Tests
{
    [Singleton]
    internal class Class1
    {
    }

    internal class Class2
    {
    }

    [TestClass]
    public class StarterTests : BaseTest
    {
        [TestMethod]
        public void Build_Singleton()
        {
            // Arrange
            this.StartAllServices<Starter>();

        // Act
            var singleton1 = Starter.Get.Resolve<Class1>();
            var singleton2 = Starter.Get.Resolve<Class1>();
            var nonSingleton1 = Starter.Get.Resolve<Class2>();
            var nonSingleton2 = Starter.Get.Resolve<Class2>();
            
            //Assert
            singleton1.Should().BeSameAs(singleton2);
            nonSingleton1.Should().NotBeSameAs(nonSingleton2);
        }

        [TestMethod]
        public void Build_WithArgs_SetEnvironmentVariables()
        {
            // Arrange
            this.Arguments.Add("--single");
            this.Arguments.Add("--value1=abcd");
            this.Arguments.Add("--value2=abcd 1234");

            // Act
            this.StartAllServices<Starter>();

            //Assert
            Environment.GetEnvironmentVariable("SINGLE").Should().Be("true");
            Environment.GetEnvironmentVariable("VALUE1").Should().Be("abcd");
            Environment.GetEnvironmentVariable("VALUE2").Should().Be("abcd 1234");
        }
        
        [TestMethod]
        public void Build_WithArgs_SetLogLevel()
        {
            // Arrange
            this.Arguments.Add("--verbose");

            this.StartAllServices<Starter>();
            StringBuilder builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            Console.SetOut(writer);
            
            // Act
            var logger = Starter.Get.Resolve<ILoggerFactory>()
                .CreateLogger("TestLogger");
            logger.LogInformation("Information Message");
            logger.LogError("Error Message");
            logger.LogDebug("Debug Message");
            logger.LogTrace("Trace Message");

            //Assert
            var result = builder.ToString();
            result.Should().Contain("Information Message");
            result.Should().Contain("Error Message");
            result.Should().Contain("Debug Message");
            result.Should().Contain("Trace Message");
        }
    }
}