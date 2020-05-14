using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor;
using GuestActionsProcessor.Domain.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]
namespace GuestActionsProcessor
{
    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration { get; private set; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            InitializeConfiguration(builder);

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            builder.Services.AddSingleton(
                s => new EventHubProducerClient(
                    Configuration[nameof(EventHubSettings) + ":" + nameof(EventHubSettings.ConnectionString)],
                    Configuration[nameof(EventHubSettings) + ":" + nameof(EventHubSettings.HubName)]));

            builder.Services.AddSingleton(
                s => new CosmosClient(
                    Configuration[nameof(CosmosSettings) + ":" + nameof(CosmosSettings.ConnectionString)]));
        }

        private void InitializeConfiguration(IFunctionsHostBuilder builder)
        {
            var executionContextOptions = builder
                .Services
                .BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>()
                .Value;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}