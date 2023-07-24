using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtectApp.Core.Models;
using PublicApi.Services;
using PublicAPI.Core.Interfaces;
using PublicAPI.Core.Models;
using System.IO;
using System.Xml.Linq;

[assembly: FunctionsStartup(typeof(PublicApi.Startup))]

namespace PublicApi
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IAzureHttpService, AzureHttpService>();
            builder.Services.AddOptions<AzureAdSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AzureAd").Bind(settings);
            });

            builder.Services.AddOptions<AzureClientCredentialSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AzureClientCredential").Bind(settings);
            });
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
