using System;
using System.Collections.Generic;
using System.Text;
using BookingApi.Application.DTOs;

namespace BookingApi.Application.Services
{
    public interface IChallengeEngineService
    {
        Task<List<ChallengeEvaluationResultDto>> EvaluateAllUsersAsync(DateTime asOfDate);
        Task<List<ChallengeAwardDto>> GetUserChallengeAwardsAsync(string userId, DateTime asOfDate);
    }
}
