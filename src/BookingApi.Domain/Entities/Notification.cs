using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities
{
    public class Notification
    {
        public string NotificationId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "CHALLENGE" or "BADGE"
        public string SourceRef { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
