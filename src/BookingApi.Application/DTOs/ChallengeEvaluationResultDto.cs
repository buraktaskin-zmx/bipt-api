using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{

    public class ChallengeEvaluationResultDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> TriggeredChallenges { get; set; } = new();
        public string? SelectedChallenge { get; set; }
        public int PointsAwarded { get; set; }
        public List<string> SuppressedChallenges { get; set; } = new();
    }
}
