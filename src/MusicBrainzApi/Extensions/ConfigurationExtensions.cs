using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using System;
using MusicBrainzApi.Core;

namespace MusicBrainzApi.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void WithKeyVaultSecrets(this ApplicationConfiguration appSettings, IConfiguration configuration)
        {
            appSettings.ConnectionStrings.RedisConnection = configuration.GetValue<string>(SecretKeyNames.RedisConnectionString);
        }

        public static X509Certificate2 WithKeyVaultCertificate(this IConfiguration configuration, string certificateName)
        {
            var certificateBundle = configuration.GetValue<string>(certificateName);
            if (certificateBundle == null)
            {
                return null;
            }

            byte[] rawSecret = Convert.FromBase64String(certificateBundle);

            return new X509Certificate2(rawSecret);
        }
    }

}
