using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities
{
    public class ChallengeAward
    {
        public string AwardId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ChallengeId { get; set; } = string.Empty;
        public DateTime AsOfDate { get; set; }
        public bool IsSelected { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
        public Challenge? Challenge { get; set; }
    }
}
