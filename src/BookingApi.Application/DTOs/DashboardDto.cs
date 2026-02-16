using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{

    public class DashboardDto
    {
        public UserDto User { get; set; } = new();
        public int TotalPoints { get; set; }
        public int LeaderboardRank { get; set; }
        public List<BadgeDto> Badges { get; set; } = new();
        public List<PointsActivityDto> RecentActivity { get; set; } = new();
    }
}
