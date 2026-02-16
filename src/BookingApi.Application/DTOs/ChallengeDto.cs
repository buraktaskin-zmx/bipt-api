using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{

    public class ChallengeDto
    {
        public string ChallengeId { get; set; } = string.Empty;
        public string ChallengeName { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }
}
