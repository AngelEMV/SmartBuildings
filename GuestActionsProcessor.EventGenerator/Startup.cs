using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor.Domain.Settings;
using GuestActionsProcessor.EventGenerator;
using GuestActionsProcessor.EventGenerator.Helpers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]
namespace GuestActionsProcessor.EventGenerator
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

            builder.Services.AddSingleton<IGuestModelsGenerator, GuestModelsGenerator>();

            builder.Services.AddSingleton(
                s => new EventHubProducerClient(
                    Configuration[nameof(EventHubSettings) + ":" + nameof(EventHubSettings.ConnectionString)],
                    Configuration[nameof(EventHubSettings) + ":" + nameof(EventHubSettings.HubName)]));
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
