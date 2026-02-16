using BookingApi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/badges")]
public class BadgeEvaluationController : ControllerBase
{
    private readonly IBadgeService _badgeService;

    public BadgeEvaluationController(IBadgeService badgeService)
    {
        _badgeService = badgeService;
    }

    /// <summary>
    /// Tüm kullanıcılar için badge'leri değerlendirir ve otomatik verir
    /// </summary>
    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateBadges()
    {
        var results = await _badgeService.EvaluateAllUsersAsync();

        var summary = new
        {
            totalUsers = results.Count,
            usersWithNewBadges = results.Count(r => r.NewBadges.Any()),
            totalNewBadges = results.Sum(r => r.NewBadges.Count),
            results = results
        };

        return Ok(summary);
    }

    /// <summary>
    /// Belirli bir kullanıcı için badge'leri değerlendirir
    /// </summary>
    [HttpPost("evaluate/{userId}")]
    public async Task<IActionResult> EvaluateUserBadges(string userId)
    {
        var result = await _badgeService.EvaluateUserAsync(userId);

        if (result.UserName == "Unknown")
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        return Ok(result);
    }
}
