using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System;

namespace MusicBrainzApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsProduction())
            {
                builder.Configuration.AddAzureKeyVault(
                    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
                    new DefaultAzureCredential());
            }

            var app = builder.Build();
            app.MapGet("/", (IConfiguration config) =>
            string.Join(
                    Environment.NewLine,
                    "SecretName (Name in Key Vault: 'SecretName')",
                    @"Obtained from configuration with config[""SecretName""]",
                    $"Value: {config["SecretName"]}",
                    "",
                    "Section:SecretName (Name in Key Vault: 'Section--SecretName')",
                    @"Obtained from configuration with config[""Section:SecretName""]",
                    $"Value: {config["Section:SecretName"]}",
                    "",
                    "Section:SecretName (Name in Key Vault: 'Section--SecretName')",
                    @"Obtained from configuration with config.GetSection(""Section"")[""SecretName""]",
                    $"Value: {config.GetSection("Section")["SecretName"]}"));

            app.Run();
        }
    }
}
