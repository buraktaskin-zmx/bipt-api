using System;
using System.Collections.Generic;
using System.Text;
using BookingApi.Application.DTOs;

namespace BookingApi.Application.Services
{
    public interface IBadgeService
    {
        Task<List<BadgeEvaluationResultDto>> EvaluateAllUsersAsync();
        Task<BadgeEvaluationResultDto> EvaluateUserAsync(string userId);
    }
}
