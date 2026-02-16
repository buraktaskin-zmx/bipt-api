using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs.AuthDTOs
{
    public class LeaderboardDto
    {
        public int Rank { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
    }
}
