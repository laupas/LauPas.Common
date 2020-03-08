using System.Threading.Tasks;

namespace LauPas.Azure
{
    public interface IAzureVault
    {
        Task<string> GetSecretAsync(string secretKey);
    }
}
