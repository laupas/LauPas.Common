using System.Threading;
using Common.Tests;
using FluentAssertions;
using LauPas.Azure;
using LauPas.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Tests
{
    [TestClass]
    public class AzureServiceBusTests : BaseTest
    {
        // [TestMethod]
        public void Queue_OneConsumer_SendGetMessage()
        {
            // Arrange
            this.StartAllServices<AzureModule>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile(".config.yml");
            var data = string.Empty;

            // Act
            Starter.Get.Resolve<IAzureServiceBus>().ListenToQueue("integration-test-queue-laupas-common", d => { data = d; });
            Starter.Get.Resolve<IAzureServiceBus>().SendToQueueAsync("integration-test-queue-laupas-common", "testData").Wait();
            
            Thread.Sleep(1000);

            //Assert
            data.Should().Be("testData");
        }
        
        // [TestMethod]
        public void Topic_TwoConsumer_SendGetMessage()
        {
            // Arrange
            this.StartAllServices<AzureModule>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile(".config.yml");
            var data1 = string.Empty;
            var data2 = string.Empty;

            // Act
            Starter.Get.Resolve<IAzureServiceBus>().ListenToTopic("integration-test-topic-laupas-common", "topic1", d => { data1 = d; });
            Starter.Get.Resolve<IAzureServiceBus>().ListenToTopic("integration-test-topic-laupas-common", "topic2", d => { data2 = d; });
            Starter.Get.Resolve<IAzureServiceBus>().SendToTopicAsync("integration-test-topic-laupas-common", "testData").Wait();

            Thread.Sleep(1000);

            //Assert
            data1.Should().Be("testData");
            data2.Should().Be("testData");
        }

    }
}