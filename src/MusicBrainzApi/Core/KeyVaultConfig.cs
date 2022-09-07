namespace MusicBrainzApi.Core
{
    public class KeyVaultConfig
    {
        public string KeyVaultUrl { get; set; }

        public bool ServerCertificateValidationEnabled { get; set; }

        public bool Enabled { get; set; }
    }
}
