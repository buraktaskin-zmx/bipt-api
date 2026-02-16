using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities
{
    public class Challenge
    {
        public string ChallengeId { get; set; } = string.Empty;
        public string ChallengeName { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty; // "messages_today >= 20"
        public int RewardPoints { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }
}
