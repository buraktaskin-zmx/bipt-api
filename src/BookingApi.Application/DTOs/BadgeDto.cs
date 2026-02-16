using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class BadgeDto
    {
        public string BadgeId { get; set; } = string.Empty;
        public string BadgeName { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int Level { get; set; }
    }
}
