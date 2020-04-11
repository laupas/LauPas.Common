using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LauPas.AnsibleVault.Tests
{
    [TestClass]
    public class AnsibleVaultTests
    {
        private IAnsibleVault ansibleVault;

        public AnsibleVaultTests()
        {
            this.ansibleVault = new LauPas.AnsibleVault.AnsibleVault();            
        }
        
        [TestMethod]
        public void DencryptValue_EncryptedString()
        {
            // Arrange
            var input = "$ANSIBLE_VAULT;1.1;AES256" + Environment.NewLine +
                                "35633533626266323031313262623432313064623134333361393937656630303664353862613963"+ Environment.NewLine +
                                "3937653935653733633665393732666130396362333232380a376461666434643738353030666339"+ Environment.NewLine +
                                "64653038663131383535336262356135656461333237623165373063393463626438616463313266"+ Environment.NewLine +
                                "6338626365336463360a383239653462653734383964666566356464346535356531626136636164"+ Environment.NewLine +
                                "6161";

            // Act
            var encryptedValue = this.ansibleVault.Decode("1234", input);
            
            // Assert
            encryptedValue.Should().Be("abcd\n");
        }
        
        [TestMethod]
        public void DecryptValue_EncryptedFile()
        {
            // Arrange
            var input = "$ANSIBLE_VAULT;1.1;AES256" + Environment.NewLine +
                                "33643238353032653563323633343066643261306333613130316266323135663864303465333063" + Environment.NewLine +
                                "3030393132643338353237366462346634666566376262610a323838616631383065323030326565" + Environment.NewLine +
                                "32353839323366323139303232343032366236373064613933663062663830616330353631646462" + Environment.NewLine +
                                "3738643964393831650a383261633135376166626437356234643764363537363533393637343164" + Environment.NewLine +
                                "64643231323565396135353962633966623361343030623464633436666432353462";

            // Act
            var encryptedValue = this.ansibleVault.Decode("1234", input);
            
            // Assert
            encryptedValue.Should().Be("value1: abcd\nvalue2: 1234\n");

        }
        
        [TestMethod]
        public void EncryptValue_EncryptedString()
        {
            // Arrange
            var input = "abcd\n";

            // Act
            var decryptedValue = this.ansibleVault.Encode("1234", input);
            
            // Assert
            var result = new LauPas.AnsibleVault.AnsibleVault().Decode("1234", decryptedValue);
            result.Should().Be(input);
        }

    }
}