using Azure.Security.KeyVault.Secrets;

namespace CustomerService.Common.Models.Configuration
{
    public class KeyVaultSetting
    {
        public SecretClient SecretClient { get; set; }  
    }
}
