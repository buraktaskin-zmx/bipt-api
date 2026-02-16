using BookingApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/challenges")]
public class ChallengeEngineController : ControllerBase
{
    private readonly IChallengeEngineService _challengeEngineService;

    public ChallengeEngineController(IChallengeEngineService challengeEngineService)
    {
        _challengeEngineService = challengeEngineService;
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateChallenges([FromQuery] DateTime asOfDate)
    {
        var results = await _challengeEngineService.EvaluateAllUsersAsync(asOfDate);
        return Ok(results);
    }

    [HttpGet("awards/{userId}")]
    public async Task<IActionResult> GetUserChallengeAwards(string userId, [FromQuery] DateTime asOfDate)
    {
        var awards = await _challengeEngineService.GetUserChallengeAwardsAsync(userId, asOfDate);
        return Ok(awards);
    }
}