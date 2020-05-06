﻿using GuestActionsProcessor.Models.SettingsModels;
using GuestActionsProcessor.Test;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Startup))]
namespace GuestActionsProcessor.Test
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            builder.Services.AddOptions<EventHubSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("EventHubSettings").Bind(settings);
                });
        }
    }
}