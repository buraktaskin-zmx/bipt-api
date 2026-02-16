using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class ChallengeAwardDto
    {
        public string AwardId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ChallengeId { get; set; } = string.Empty;
        public string ChallengeName { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
        public bool IsSelected { get; set; }
        public DateTime AsOfDate { get; set; }
    }
}
