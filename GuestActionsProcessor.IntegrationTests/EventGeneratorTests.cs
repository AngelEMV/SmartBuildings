using Azure.Messaging.EventHubs.Producer;
using GuestActionsProcessor.EventGenerator.Functions;
using GuestActionsProcessor.EventGenerator.Helpers;
using GuestActionsProcessor.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace GuestActionsProcessor.IntegrationTests
{
    [Collection(TestsCollection.Name)]
    public class EventGeneratorTests
    {
        readonly EventGeneratorFunction _sut;

        public EventGeneratorTests(TestHost testHost)
        {
            _sut = new EventGeneratorFunction(
                testHost.ServiceProvider.GetRequiredService<ILogger<EventGeneratorFunction>>(),
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
    }
}
