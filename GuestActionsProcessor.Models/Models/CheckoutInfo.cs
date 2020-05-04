using System;

namespace GuestActionsProcessor.Models.Models
{
    public class CheckoutInfo
    {
        public string BuildingId { get; set; }
        public string UserId { get; set; }
        public DateTime CheckoutDateTime { get; set; }
    }
}
