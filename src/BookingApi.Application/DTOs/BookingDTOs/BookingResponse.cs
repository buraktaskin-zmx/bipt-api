using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Application.DTOs.BookingDTOs;

public record BookingResponse(
    Guid Id,
    string Service,
    DateTime StartTime,
    DateTime EndTime,
    string Status,
    DateTime CreatedAt
);