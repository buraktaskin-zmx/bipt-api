using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities
{
    public class Badge
    {
        public string BadgeId { get; set; } = string.Empty;
        public string BadgeName { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty; // "total_points >= 200"
        public int Level { get; set; }
    }
}
