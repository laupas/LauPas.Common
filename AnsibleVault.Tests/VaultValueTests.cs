using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LauPas.AnsibleVault.Tests
{
    [TestClass]
    public class VaultValueTests
    {
        [TestMethod]
        public void Ctor_FromEncryptedValue()
        {
            // Arrange
            var input = "$ANSIBLE_VAULT;1.1;AES256" + Environment.NewLine +
                        "33343835306666636239373663396363643766613363343837646633343933376633323964663030" + Environment.NewLine +
                        "3134616235646661306436643134383333633730376233650a663466323032343633383061336461" + Environment.NewLine +
                        "36393261363338616337613039363435313631343437323164386661326633313339396238396236" + Environment.NewLine +
                        "3462393338636632650a653036663266373533343232393838343161396564333963643632653932" + Environment.NewLine +
                        "30386135636131656130346537356637396139323134386162306431376564346537633566666532" + Environment.NewLine +
                        "6331323061373237336639356165393563613765663864366231";

            // Act
            var value = new VaultValue(input);
            
            // Assert
            value.Salt.Should()
                .BeEquivalentTo("34850ffcb976c9ccd7fa3c487df34937f329df0014ab5dfa0d6d14833c707b3e".ConvertHexStringToBytes());
            value.Hamc.Should()
                .BeEquivalentTo("f4f20246380a3da692a638ac7a0964516144721d8fa2f31399b89b64b938cf2e".ConvertHexStringToBytes());
            value.Body.Should()
                .BeEquivalentTo("e06f2f75342298841a9ed39cd62e9208a5ca1ea04e75f79a92148ab0d17ed4e7c5ffe2c120a7273f95ae95ca7ef8d6b1".ConvertHexStringToBytes());
        }

        [TestMethod]
        public void Ctor_CreateNewValue()
        {
            // Arrange
            
            // Act
            var value = new VaultValue();

            //Assert
            value.Header.Should().Be("$ANSIBLE_VAULT;1.1;AES256");
        }
        
        [TestMethod]
        public void ToVaultString()
        {
            // Arrange
            var value = new VaultValue();
            value.Salt = "34850ffcb976c9ccd7fa3c487df34937f329df0014ab5dfa0d6d14833c707b3e".ConvertHexStringToBytes();
            value.Hamc = "f4f20246380a3da692a638ac7a0964516144721d8fa2f31399b89b64b938cf2e".ConvertHexStringToBytes();
            value.Body = "e06f2f75342298841a9ed39cd62e9208a5ca1ea04e75f79a92148ab0d17ed4e7c5ffe2c120a7273f95ae95ca7ef8d6b1".ConvertHexStringToBytes();

            // Act
            var result = value.ToVaultString();
            
            // Assert
            var expected = "$ANSIBLE_VAULT;1.1;AES256" + Environment.NewLine +
                         "33343835306666636239373663396363643766613363343837646633343933376633323964663030" + Environment.NewLine +
                         "3134616235646661306436643134383333633730376233650a663466323032343633383061336461" + Environment.NewLine +
                         "36393261363338616337613039363435313631343437323164386661326633313339396238396236" + Environment.NewLine +
                         "3462393338636632650a653036663266373533343232393838343161396564333963643632653932" + Environment.NewLine +
                         "30386135636131656130346537356637396139323134386162306431376564346537633566666532" + Environment.NewLine +
                         "6331323061373237336639356165393563613765663864366231";

             result.Should().Be(expected);
        }
    }
}