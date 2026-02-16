using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities
{
    public class BadgeAward
    {
        public string AwardId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string BadgeId { get; set; } = string.Empty;
      

        // Navigation
        public User? User { get; set; }
        public Badge? Badge { get; set; }
    }
}
