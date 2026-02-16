using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities
{
    public class PointsLedger
    {
        public string LedgerId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int PointsDelta { get; set; }
        public string Source { get; set; } = string.Empty;
        public string SourceRef { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}
