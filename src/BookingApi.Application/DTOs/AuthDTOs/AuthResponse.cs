using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs.AuthDTOs
{
    public record AuthResponse(string Token, string Email, string DisplayName);

}
