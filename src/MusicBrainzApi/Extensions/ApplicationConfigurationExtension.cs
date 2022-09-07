using Microsoft.Extensions.Configuration;
using MusicBrainzApi.Core;

namespace MusicBrainzApi.Extensions
{
    public static class ApplicationConfigurationExtension
    {
        public static ApplicationConfiguration ApplicationConfiguration(this IConfiguration configuration)
        {
            var applicationConfiguration = configuration.Get<ApplicationConfiguration>();
            if (applicationConfiguration.KeyVault.Enabled)
            {
                applicationConfiguration.WithKeyVaultSecrets(configuration);
            }
            return applicationConfiguration;
        }
    }

}
