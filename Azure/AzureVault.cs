using System;
using System.Threading.Tasks;
using LauPas.AnsibleVault;
using LauPas.Azure.Model;
using LauPas.Common;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace LauPas.Azure
{
    [Singleton]
    internal class AzureVault : IAzureVault
    {
        private readonly ILogger logger;
        private readonly KeyVaultClient keyVaultClient;
        private readonly string vaultUri;
        
        public AzureVault(ILoggerFactory loggerFactory, IConfigService configService)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
            var config = configService.Get<AzureVaultConfiguration>("AzureVaultConfiguration");
            this.vaultUri = config.VaultUrl;

            var clientCred = new ClientCredential(config.ClientId, config.ClientSecret);
            this.keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
            {
                this.logger.LogTrace($"Get Token for: {this.vaultUri}");

                var authContext = new AuthenticationContext(authority);
                var result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                {
                    throw new InvalidOperationException("Failed to obtain the JWT token");
                }

                return result.AccessToken;
            });
        }

        public async Task<T> GetSecretAsync<T>(string secretKey, string password = null)
        {
            this.logger.LogDebug($"GetSecretAsync: {secretKey}");
            var secretValue = await this.keyVaultClient.GetSecretAsync(this.vaultUri, secretKey);
            if (password == null)
            {
                return secretValue.Value.Deserialize<T>();
            }
            else
            {
                var ansibleVault = Starter.Get.Resolve<IAnsibleVault>();
                return ansibleVault.Decode(password, secretValue.Value).Deserialize<T>();
            }
        }

        public Task SetSecretAsync<T>(string secretKey, T data, string password = null)
        {
            this.logger.LogDebug($"SetSecretAsync: {secretKey}");
            if (password == null)
            {
                return this.keyVaultClient.SetSecretAsync(this.vaultUri, secretKey, data.Serialize());
            }
            else
            {
                var ansibleVault = Starter.Get.Resolve<IAnsibleVault>();
                return this.keyVaultClient.SetSecretAsync(
                    this.vaultUri, 
                    secretKey,
                    ansibleVault.Encode(password, data.Serialize()));
            }
        }
    }
}