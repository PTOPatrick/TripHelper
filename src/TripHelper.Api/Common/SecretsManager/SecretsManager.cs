using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace AdminCenter.Application.Common.Secrets
{
    public class SecretsManager
    {
        private readonly KeyValuePair<string, string> Auth0ManagedApiAudience = new(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_KEY")!, "");
        private readonly List<KeyVaultSecret> _secrets = [];
        private readonly SecretClient _client;

        public SecretsManager(string keyVaultUrl)
        {
            var options = new SecretClientOptions
            {
                Retry = { Delay = TimeSpan.FromSeconds(2), MaxDelay = TimeSpan.FromSeconds(16), MaxRetries = 5, Mode = Azure.Core.RetryMode.Exponential }
            };
            
            _client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential(), options);
            
            CollectSecrets();
        }

        public string GetSecret(string key)
        {
            return _secrets.FirstOrDefault(s => s.Name == key)!.Value;
        }

        private void CollectSecrets()
        {
            _secrets.Add(_client.GetSecret(Auth0ManagedApiAudience.Key));

            if (!AreAllSecretsAreCollected())
                throw new("Secret not found");
        }

        private bool AreAllSecretsAreCollected()
        {
            return !_secrets.Any(secret => string.IsNullOrWhiteSpace(secret.Value));
        }
    }
}