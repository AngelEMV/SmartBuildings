using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor.Models.SettingsModels;
using GuestActionsProcessor.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace GuestActionsProcessor.Test.Functions
{
    public class EventGenerator
    {
        private readonly ILogger<EventGenerator> _logger;
        private readonly EventHubSettings _eventHubOptions;
        private readonly EventHubProducerClient _eventHubProducerClient;

        public EventGenerator(
            ILogger<EventGenerator> logger,
            IOptions<EventHubSettings> eventHubSettings)
        {
            _logger = logger;
            _eventHubOptions = eventHubSettings.Value;
            _eventHubProducerClient = new EventHubProducerClient(_eventHubOptions.ConnectionString, _eventHubOptions.HubName);
        }

        [FunctionName(nameof(EventGenerator))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "EventGenerate")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                var checkinInfo = GuestModelsGenerator.GenerateCheckinInfo(1);
                await SendToEventHubAsync(checkinInfo);
                _logger.LogInformation($"- User {checkinInfo.FirstName} {checkinInfo.LastName} performed Checkin. Sending CheckInInfo to Event Hub");

                var actionsInfo = GuestModelsGenerator.GenerateActionInfo(10);
                foreach (var action in actionsInfo)
                {
                    await SendToEventHubAsync(action);
                    _logger.LogInformation($"- User {action.FirstName} {action.LastName} has accesed to {action.AreaName}. Sending ActionInfo to Event Hub");
                }

                var checkoutInfo = GuestModelsGenerator.GenerateCheckoutInfo(1);
                await SendToEventHubAsync(checkoutInfo);
                _logger.LogInformation($"- User {checkoutInfo.FirstName} {checkoutInfo.LastName} performed Checkout. Sending CheckoutInfo to Event Hub");

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Something went wrong. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        private async Task SendToEventHubAsync(object payload)
        {
            EventDataBatch eventDataBatch = await _eventHubProducerClient.CreateBatchAsync();
            var eventReading = JsonConvert.SerializeObject(payload);
            eventDataBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(eventReading)));
            await _eventHubProducerClient.SendAsync(eventDataBatch);
        }
    }
}
