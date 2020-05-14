using System;

namespace GuestActionsProcessor.Domain.Models
{
    public class BuildingInfo
    {
        public Guid BuildingId { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
}
