using System.Threading.Tasks;

namespace Azure
{
    public interface IAzureVault
    {
        Task<string> GetSecretAsync(string secretKey);
    }
}
