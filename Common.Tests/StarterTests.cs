using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LauPas.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Common.Tests
{
    
    [TestClass]
    public abstract class BaseTest
    {
//        protected Mock<IProcessHelper> ProcessMock { get; set; }
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Default);
        private readonly List<Mock> mocks = new List<Mock>();

        protected void StartAllServices()
        {
            Starter.Create().AddAssembly(this).Build(this.Arguments.ToArray(), collection =>
            {
                foreach (var mock in this.mocks)
                {
                    collection
                        .Where(r => r.ServiceType == mock.GetType().GenericTypeArguments[0])
                        .ToList()
                        .ForEach(c =>
                        {
                            collection.Remove(c);
                        });
                    collection.AddSingleton(mock.GetType().GenericTypeArguments[0], mock.Object);
                }
            });
        }
        
        protected Mock<T> RegisterMock<T>() where T : class
        {
            var mock = this.CreateMock<T>();
            this.mocks.Add(mock);
            return mock;
        }

        protected Mock<T> CreateMock<T>() where T : class
        {
            var mock = this.mockRepository.Create<T>();
            return mock;
        }

        protected List<string> Arguments { get; set; } = new List<string>();

    }
    
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
            this.StartAllServices();

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
            this.StartAllServices();

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

            this.StartAllServices();
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