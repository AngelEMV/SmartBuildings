using System;

namespace GuestActionsProcessor.Models.Models
{
    public class GuestActionInfo
    {
        public string BuildingId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AreaName { get; set; }
        public DateTime ActionDateTime { get; set; }
    }
}
