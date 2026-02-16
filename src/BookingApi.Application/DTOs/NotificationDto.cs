using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{

    public class NotificationDto
    {
        public string NotificationId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "CHALLENGE" or "BADGE"
        public string SourceRef { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
