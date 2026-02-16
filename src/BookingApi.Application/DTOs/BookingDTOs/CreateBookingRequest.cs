using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs.BookingDTOs
{
    public record CreateBookingRequest(string Service, DateTime StartTime, DateTime EndTime);

}
