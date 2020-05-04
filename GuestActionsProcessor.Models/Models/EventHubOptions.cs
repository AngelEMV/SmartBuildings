using System;
using System.Collections.Generic;
using System.Text;

namespace GuestActionsProcessor.Models.Models
{
    public class EventHubSettings
    {
        public string ConnectionString { get; set; }
        public string HubName { get; set; }
    }
}