using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs
{
    public class UserDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
