using Common.Tests;
using FluentAssertions;
using LauPas.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Tests
{
    [TestClass]
    public class AzureVaultTests : BaseTest
    {
        [TestMethod]
        public void GetSecretAsync_VaildKey_GetSecret()
        {
            // Arrange
            this.StartAllServices<IAzureVault>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile(".config.yml");
            
            // Act
            var vaultValue = Starter.Get.Resolve<IAzureVault>().GetSecretAsync("test").Result;

            // Assert
            vaultValue.Should().Be("abcd");
        }
    }
}