using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class BadgeEvaluationResultDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public List<BadgeAwardResultDto> NewBadges { get; set; } = new();
        public List<string> ExistingBadges { get; set; } = new();
    }
}
