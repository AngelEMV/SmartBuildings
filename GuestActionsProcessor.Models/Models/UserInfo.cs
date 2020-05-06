using System;
using System.Collections.Generic;
using System.Text;

namespace GuestActionsProcessor.Models.Models
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
