using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor.Domain.Models;
using GuestActionsProcessor.Domain.Settings;
using GuestActionsProcessor.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuestActionsProcessor.Test.Functions
{
    public class EventGenerator
    {
        private readonly ILogger<EventGenerator> _logger;
        private readonly EventHubSettings _eventHubSettings;
        private readonly EventHubProducerClient _eventHubProducerClient;

        public EventGenerator(
            ILogger<EventGenerator> logger,
            IOptions<EventHubSettings> eventHubSettings)
        {
            _logger = logger;
            _eventHubSettings = eventHubSettings.Value;
            _eventHubProducerClient = new EventHubProducerClient(_eventHubSettings.ConnectionString, _eventHubSettings.HubName);
        }

        [FunctionName(nameof(EventGenerator))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "EventGenerate")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                var buildingInfo = GuestModelsGenerator.GenerateBuildingInfo();
                var userInfo = GuestModelsGenerator.GenerateUserInfo();

                await SendCheckinInfo(buildingInfo, userInfo);

                await SendRandomActions(buildingInfo, userInfo, 10);

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
            var checkinInfo = GuestModelsGenerator.GenerateCheckinInfo();
            var checkinActionInfo = GuestModelsGenerator.GenerateActionInfo(1, buildingInfo, userInfo)?.FirstOrDefault();
            checkinActionInfo.CheckinInfo = checkinInfo;
            checkinActionInfo.AreaName = null;
            await SendToEventHubAsync(checkinActionInfo);
            _logger.LogInformation($"---- User {userInfo?.FirstName} {userInfo?.LastName} performed Checkin. Sending CheckInInfo to Event Hub");
        }

        private async Task SendRandomActions(BuildingInfo buildingInfo, UserInfo userInfo, int amount)
        {
            var actionsInfo = GuestModelsGenerator.GenerateActionInfo(amount, buildingInfo, userInfo);
            foreach (var action in actionsInfo)
            {
                await SendToEventHubAsync(action);
                _logger.LogInformation($"---- User {action?.UserInfo?.FirstName} {action?.UserInfo?.LastName} has accesed to {action.AreaName}. Sending ActionInfo to Event Hub");
            }
        }

        private async Task SendCheckoutInfo(BuildingInfo buildingInfo, UserInfo userInfo)
        {
            var checkoutInfo = GuestModelsGenerator.GenerateCheckoutInfo();
            var checkoutActionInfo = GuestModelsGenerator.GenerateActionInfo(1, buildingInfo, userInfo)?.FirstOrDefault();
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
