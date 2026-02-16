using BookingApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLeaderboard()
    {
        var leaderboard = await _leaderboardService.GetLeaderboardAsync();
        return Ok(leaderboard);
    }
}