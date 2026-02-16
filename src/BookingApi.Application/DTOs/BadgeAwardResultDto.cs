using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class BadgeAwardResultDto
    {
        public string BadgeId { get; set; } = string.Empty;
        public string BadgeName { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsNew { get; set; } // Yeni kazanıldı mı?
    }
}
