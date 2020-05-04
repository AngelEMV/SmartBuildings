using System;

namespace GuestActionsProcessor.Models.Models
{
    public class CheckinInfo
    {
        public string BuildingId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CheckinDateTime { get; set; }
    }
}
