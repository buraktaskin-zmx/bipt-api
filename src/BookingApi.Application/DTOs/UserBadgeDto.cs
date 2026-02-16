using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class UserBadgeDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<BadgeDto> Badges { get; set; } = new();
    }
}
