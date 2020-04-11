namespace LauPas.Azure.Model
{
    /// <summary>
    /// Configuration for AzureVault
    /// </summary>
    public class AzureVaultConfiguration
    {
        /// <summary>
        /// The Azure Client ID.
        /// </summary>
        public string ClientId { get; set; }
        
        /// <summary>
        /// The Azure Clien Secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The Vault Url.
        /// </summary>
        public string VaultUrl { get; set; }
    }
}