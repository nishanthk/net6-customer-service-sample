using Azure.Security.KeyVault.Secrets;
using CustomerService.Common.Repository.Interfaces;
using System.Threading.Tasks;

namespace CustomerService.Common.Repository.Wrapper
{
    public class ClientSecretWrapper : IClientSecretWrapper
    {
        public SecretClient SecretClient { get; set; }

        public ClientSecretWrapper()
        { }

        public async Task<KeyVaultSecret> GetAsync(string subscriptionId) =>
            await SecretClient.GetSecretAsync(subscriptionId);
    }
}
