using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class ActivityEventDto
    {
        public string EventId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Messages { get; set; }
        public int Reactions { get; set; }
        public int UniqueGroups { get; set; }
    }
}
