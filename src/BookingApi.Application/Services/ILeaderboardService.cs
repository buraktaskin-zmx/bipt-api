using System;
using System.Collections.Generic;
using System.Text;
using BookingApi.Application.DTOs.AuthDTOs;

namespace BookingApi.Application.Services
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardDto>> GetLeaderboardAsync();
    }
}
