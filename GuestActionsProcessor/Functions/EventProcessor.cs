using GuestActionsProcessor.Models.Models;
using GuestActionsProcessor.Models.SettingsModels;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace GuestActionsProcessor.Functions
{
    public class EventProcessor
    {
        private readonly ILogger<EventProcessor> _logger;

        private readonly CosmosSettings _cosmosSettings;
        private CosmosClient _cosmosClient;
        private Container _cosmosContainer;

        public EventProcessor(
            ILogger<EventProcessor> logger,
            IOptions<CosmosSettings> cosmosSettings)
        {
            _logger = logger;
            _cosmosSettings = cosmosSettings.Value;
            _cosmosClient = new CosmosClient(_cosmosSettings.ConnectionString);
            _cosmosContainer = _cosmosClient.GetContainer(_cosmosSettings.DatabaseName, _cosmosSettings.ContainerName);
        }

        [FunctionName(nameof(EventProcessor))]
        public async Task Run([EventHubTrigger("guestactions", 
            Connection = "EventHubSettings:ConnectionString")] EventData[] events)
        {
            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var actionInfo = JsonConvert.DeserializeObject<GuestActionInfo>(messageBody);

                    await _cosmosContainer.CreateItemAsync(actionInfo);

                    if (actionInfo?.CheckinInfo != null)
                    {
                        _logger.LogInformation($"- Checkin Info Received from User {actionInfo?.UserInfo?.FirstName} {actionInfo?.UserInfo?.LastName}. Event Information stored in CosmosDB.");
                        _logger.LogInformation($"    (Simulated) Calling external service: Store data in Booking system");
                        _logger.LogInformation($"    (Simulated) Calling external service: Turn on IoT climate");
                    }

                    if (actionInfo?.CheckoutInfo != null)
                    {
                        _logger.LogInformation($"- Checkout Info Received from User {actionInfo?.UserInfo?.FirstName} {actionInfo?.UserInfo?.LastName}. Event Information stored in CosmosDB.");
                        _logger.LogInformation($"    (Simulated) Calling external service: Store data in Booking system");
                        _logger.LogInformation($"    (Simulated) Calling service: Revoking access permissions");
                    }

                    if (!string.IsNullOrEmpty(actionInfo?.AreaName))
                    {
                        _logger.LogInformation($"- User {actionInfo?.UserInfo?.FirstName} {actionInfo?.UserInfo?.LastName} accessed to {actionInfo?.AreaName}. Event Information stored in CosmosDB.");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Something went wrong. Exception thrown: {e.Message}");
                }
            }
        }
    }
}
