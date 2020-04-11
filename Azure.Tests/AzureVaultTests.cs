using System;
using Common.Tests;
using FluentAssertions;
using FluentAssertions.Primitives;
using LauPas.AnsibleVault;
using LauPas.Azure;
using LauPas.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azure.Tests
{
    [TestClass]
    public class AzureVaultTests : BaseTest
    {
        [TestMethod]
        public void SetGetSecretAsync_VaildKey_GetSecret()
        {
            // Arrange
            this.StartAllServices<IAzureVault>();
            Starter.Get.Resolve<IConfigService>().SetConfigFile(".config.yml");
            var value = DateTime.Now;
            
            // Act
            Starter.Get.Resolve<IAzureVault>().SetSecretAsync<DateTime>("test", value).Wait();

            // Assert
            var vaultValue = Starter.Get.Resolve<IAzureVault>().GetSecretAsync<DateTime>("test").Result;
            vaultValue.Should().Be(value);
        }

        [TestMethod]
        public void SetGetSecretAsync_VaildKey_WithPassword_GetSecret()
        {
            // Arrange
            this.StartAllServices<IAzureVault>(typeof(IAnsibleVault));
            Starter.Get.Resolve<IConfigService>().SetConfigFile(".config.yml");
            var value = DateTime.Now;
            
            // Act
            Starter.Get.Resolve<IAzureVault>().SetSecretAsync<DateTime>("test", value, "1234").Wait();

            // Assert
            var vaultValue = Starter.Get.Resolve<IAzureVault>().GetSecretAsync<DateTime>("test", "1234").Result;
            vaultValue.Should().Be(value);
        }
        
    }
}