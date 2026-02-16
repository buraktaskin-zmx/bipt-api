using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class PointsActivityDto
    {
        public string LedgerId { get; set; } = string.Empty;
        public int PointsDelta { get; set; }
        public string Source { get; set; } = string.Empty;
        public string SourceRef { get; set; } = string.Empty;
    }
}
