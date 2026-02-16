using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities
{
    public class UserState
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime AsOfDate { get; set; }
        public int MessagesToday { get; set; }
        public int ReactionsToday { get; set; }
        public int UniqueGroupsToday { get; set; }
        public int Messages7d { get; set; }
        public int Reactions7d { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
