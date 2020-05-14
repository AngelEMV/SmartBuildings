using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor.EventGenerator.Functions;
using GuestActionsProcessor.EventGenerator.Helpers;
using GuestActionsProcessor.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Xunit;

namespace GuestActionsProcessor.IntegrationTests
{
    [Collection(TestsCollection.Name)]
    public class EventGeneratorTests
    {
        private readonly EventGeneratorFunction _sut;
        private readonly ILogger<EventGeneratorFunction> _logger;

        public EventGeneratorTests(TestHost testHost)
        {
            _logger = TestFactory.CreateTypedLogger();

            _sut = new EventGeneratorFunction(
                _logger,
                testHost.ServiceProvider.GetRequiredService<IGuestModelsGenerator>(),
                testHost.ServiceProvider.GetRequiredService<EventHubProducerClient>());
        }

        [Fact]
        public async void EventGenerator_should_return_ok_status_response()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());
            var response = (OkResult)await _sut.Run(request);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        [Fact]
        public async void EventGenerator_should_perform_certain_number_of_actions_when_request_query_is_empty()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());
            await _sut.Run(request);

            var logs = ((ListLogger)_logger).Logs;
            Assert.Equal(12, logs.Count);
        }

        [Fact]
        public async void EventGenerator_should_perform_certain_number_of_actions()
        {
            var request = TestFactory.CreateHttpRequest("amount", "4");
            await _sut.Run(request);

            var logs = ((ListLogger)_logger).Logs;
            Assert.Equal(6, logs.Count);
        }

        [Fact]
        public async void EventGenerator_should_send_checkin_info_at_the_beginning()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());
            await _sut.Run(request);

            var logs = ((ListLogger)_logger).Logs;
            Assert.Contains("Sending CheckInInfo to Event Hub", logs.First());
        }

        [Fact]
        public async void EventGenerator_should_send_action_info_a_certain_number_of_times()
        {
            var request = TestFactory.CreateHttpRequest("amount", "4");
            await _sut.Run(request);
            var logs = ((ListLogger)_logger).Logs;
            var numberOfActionsSent = logs.Where(s => s.Contains("Sending ActionInfo to Event Hub")).Count();
            Assert.Equal(4, numberOfActionsSent);
        }

        [Fact]
        public async void EventGenerator_should_send_checkout_info_at_the_end()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());
            await _sut.Run(request);

            var logs = ((ListLogger)_logger).Logs;
            Assert.Contains("Sending CheckoutInfo to Event Hub", logs.Last());
        }
    }
}
