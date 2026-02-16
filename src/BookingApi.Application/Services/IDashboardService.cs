using System;
using System.Collections.Generic;
using System.Text;
using BookingApi.Application.DTOs;

namespace BookingApi.Application.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto?> GetUserDashboardAsync(string userId);
    }
}
