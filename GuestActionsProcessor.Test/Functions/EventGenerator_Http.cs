using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor.Models.Models;
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
    public class EventGenerator_Http
    {
        private readonly ILogger<EventGenerator_Http> _logger;
        private readonly EventHubSettings _eventHubOptions;
        private readonly EventHubProducerClient _eventHubProducerClient;

        public EventGenerator_Http(
            ILogger<EventGenerator_Http> logger,
            IOptions<EventHubSettings> eventHubSettings)
        {
            _logger = logger;
            _eventHubOptions = eventHubSettings.Value;
            _eventHubProducerClient = new EventHubProducerClient(_eventHubOptions.ConnectionString, _eventHubOptions.HubName);
        }

        [FunctionName(nameof(EventGenerator_Http))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "EventGenerate")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                var checkinInfo = GuestModelsGenerator.GenerateCheckinInfo(1);
                await SendToEventHubAsync(checkinInfo);
                _logger.LogInformation($"Sending {checkinInfo.FirstName} {checkinInfo.LastName} CheckInInfo to Event Hub");

                var checkoutInfo = GuestModelsGenerator.GenerateCheckinInfo(1);
                await SendToEventHubAsync(checkinInfo);
                _logger.LogInformation($"Sending {checkinInfo.FirstName} {checkinInfo.LastName} CheckoutInfo to Event Hub");

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
