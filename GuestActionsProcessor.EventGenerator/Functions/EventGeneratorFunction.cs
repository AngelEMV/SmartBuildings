using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor.Domain.Models;
using GuestActionsProcessor.EventGenerator.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuestActionsProcessor.EventGenerator.Functions
{
    public class EventGeneratorFunction
    {
        private readonly ILogger<EventGeneratorFunction> _logger;
        private readonly IGuestModelsGenerator _guestModelsGenerator;
        private readonly EventHubProducerClient _eventHubProducerClient;

        public EventGeneratorFunction(
            ILogger<EventGeneratorFunction> logger,
            IGuestModelsGenerator guestModelsGenerator,
            EventHubProducerClient eventHubProducerClient)
        {
            _logger = logger;
            _eventHubProducerClient = eventHubProducerClient;
            _guestModelsGenerator = guestModelsGenerator;
        }

        [FunctionName(nameof(EventGenerator))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "EventGenerate")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                int amountOfEventsToGenerate = int.TryParse(req.Query["amount"], out amountOfEventsToGenerate) ? amountOfEventsToGenerate : 10;

                var buildingInfo = _guestModelsGenerator.GenerateBuildingInfo();
                var userInfo = _guestModelsGenerator.GenerateUserInfo();

                await SendCheckinInfo(buildingInfo, userInfo);

                await SendRandomActions(buildingInfo, userInfo, amountOfEventsToGenerate);

                await SendCheckoutInfo(buildingInfo, userInfo);

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Something went wrong. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }

        private async Task SendCheckinInfo(BuildingInfo buildingInfo, UserInfo userInfo)
        {
            var checkinInfo = _guestModelsGenerator.GenerateCheckinInfo();
            var checkinActionInfo = _guestModelsGenerator.GenerateActionInfo(1, buildingInfo, userInfo)?.FirstOrDefault();
            checkinActionInfo.CheckinInfo = checkinInfo;
            checkinActionInfo.AreaName = null;
            await SendToEventHubAsync(checkinActionInfo);
            _logger.LogInformation($"---- User {userInfo?.FirstName} {userInfo?.LastName} performed Checkin. Sending CheckInInfo to Event Hub");
        }

        private async Task SendRandomActions(BuildingInfo buildingInfo, UserInfo userInfo, int amount)
        {
            var actionsInfo = _guestModelsGenerator.GenerateActionInfo(amount, buildingInfo, userInfo);
            foreach (var action in actionsInfo)
            {
                await SendToEventHubAsync(action);
                _logger.LogInformation($"---- User {action?.UserInfo?.FirstName} {action?.UserInfo?.LastName} has accesed to {action.AreaName}. Sending ActionInfo to Event Hub");
            }
        }

        private async Task SendCheckoutInfo(BuildingInfo buildingInfo, UserInfo userInfo)
        {
            var checkoutInfo = _guestModelsGenerator.GenerateCheckoutInfo();
            var checkoutActionInfo = _guestModelsGenerator.GenerateActionInfo(1, buildingInfo, userInfo)?.FirstOrDefault();
            checkoutActionInfo.CheckinInfo = null;
            checkoutActionInfo.AreaName = null;
            checkoutActionInfo.CheckoutInfo = checkoutInfo;
            await SendToEventHubAsync(checkoutActionInfo);
            _logger.LogInformation($"---- User {userInfo?.FirstName} {userInfo?.LastName} performed Checkout. Sending CheckoutInfo to Event Hub");
        }

        private async Task SendToEventHubAsync(GuestActionInfo payload)
        {
            EventDataBatch eventDataBatch = await _eventHubProducerClient.CreateBatchAsync();
            var eventReading = JsonConvert.SerializeObject(payload);
            eventDataBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(eventReading)));
            await _eventHubProducerClient.SendAsync(eventDataBatch);
        }
    }
}
