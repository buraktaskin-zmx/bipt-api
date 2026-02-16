using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs.AuthDTOs
{
    public record RegisterRequest(string Email, string Password, string DisplayName);

}
