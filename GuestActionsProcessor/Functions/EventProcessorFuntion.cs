using GuestActionsProcessor.Domain.Models;
using GuestActionsProcessor.Domain.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace GuestActionsProcessor.Functions
{
    public class EventProcessorFuntion
    {
        private readonly ILogger<EventProcessorFuntion> _logger;

        private readonly CosmosClient _cosmosClient;
        private readonly Container _cosmosContainer;

        public EventProcessorFuntion(
            ILogger<EventProcessorFuntion> logger,
            IConfiguration config,
            CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;

            _cosmosContainer = _cosmosClient.GetContainer(
                config[nameof(CosmosSettings) + ":" + nameof(CosmosSettings.DatabaseName)],
                config[nameof(CosmosSettings) + ":" + nameof(CosmosSettings.ContainerName)]);
        }

        [FunctionName(nameof(EventProcessorFuntion))]
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
