using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs.AuthDTOs
{
    public record LoginRequest(string Email, string Password);

}
