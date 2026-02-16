using System;
using System.Collections.Generic;
using System.Text;
using BookingApi.Application.DTOs;

namespace BookingApi.Infrastructure.Services
{
    public interface IMetricsService
    {
        Task<UserStateDto?> GetUserMetricsAsync(string userId, DateTime asOfDate);
        Task<List<UserStateDto>> CalculateMetricsAsync(DateTime asOfDate);
    }
}
