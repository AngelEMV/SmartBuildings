using System;

namespace GuestActionsProcessor.Domain.Models
{
    public class GuestActionInfo
    {
        public BuildingInfo BuildingInfo { get; set; }
        public UserInfo UserInfo { get; set; }
        public CheckinInfo CheckinInfo { get; set; }
        public CheckoutInfo CheckoutInfo { get; set; }

        public Guid id { get; set; }
        public string AreaName { get; set; }
        public DateTime ActionDateTime { get; set; }
    }
}
