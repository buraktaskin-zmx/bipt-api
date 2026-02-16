using BookingApi.Application.DTOs;
using BookingApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BadgesController : ControllerBase
{
    private readonly AppDbContext _context;

    public BadgesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBadges()
    {
        var badges = await _context.Badges
            .Select(b => new BadgeDto
            {
                BadgeId = b.BadgeId,
                BadgeName = b.BadgeName,
                Condition = b.Condition,
                Level = b.Level
            })
            .OrderBy(b => b.Level)
            .ToListAsync();

        return Ok(badges);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBadges(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound();

        var userBadges = await _context.BadgeAwards
            .Where(ba => ba.UserId == userId)
            .Include(ba => ba.Badge)
            .Select(ba => new BadgeDto
            {
                BadgeId = ba.Badge!.BadgeId,
                BadgeName = ba.Badge.BadgeName,
                Condition = ba.Badge.Condition,
                Level = ba.Badge.Level
            })
            .OrderBy(b => b.Level)
            .ToListAsync();

        var result = new UserBadgeDto
        {
            UserId = user.UserId,
            UserName = user.Name,
            Badges = userBadges
        };

        return Ok(result);
    }
}
