using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using MusicBrainzApi.Core;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace MusicBrainzApi
{
    public class Program
    {
        private static readonly KeyVaultClient KeyVaultClient = SetupKeyVaultClient();
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) => { ConfigureConfigurationBuilder(builder); })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        private static void ConfigureConfigurationBuilder(IConfigurationBuilder configBuilder)
        {
            var appSettings = configBuilder.Build().Get<ApplicationConfiguration>();

            if (appSettings.KeyVault.Enabled)
            {
                configBuilder.AddAzureKeyVault(
                    appSettings.KeyVault.KeyVaultUrl,
                    KeyVaultClient,
                    new DefaultKeyVaultSecretManager());
            }
        }

        private static KeyVaultClient SetupKeyVaultClient()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            return new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback));
        }


    }
}
