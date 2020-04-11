using System.Threading.Tasks;

namespace LauPas.Azure
{
    public interface IAzureVault
    {
        /// <summary>
        /// Read a value from Azure Vault.
        /// </summary>
        /// <param name="secretKey">The secret name</param>
        /// <param name="password">If not null, IAnsibleVault will be used to Encode the vaule</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetSecretAsync<T>(string secretKey, string password = null);
        
        /// <summary>
        /// Write a Value to Azure Vault.
        /// </summary>
        /// <param name="secretKey">The secret name</param>
        /// <param name="data">The data to write</param>
        /// <param name="password"></param>
        /// <typeparam name="T">If not null, IAnsibleVault will be used to Encode the vaule</typeparam>
        /// <returns></returns>
        Task SetSecretAsync<T>(string secretKey, T data, string password = null);
    }
}
