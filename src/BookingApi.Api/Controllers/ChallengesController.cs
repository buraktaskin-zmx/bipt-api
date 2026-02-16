using BookingApi.Application.DTOs;
using BookingApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChallengesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChallengesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetChallenges()
    {
        var challenges = await _context.Challenges
            .Where(c => c.IsActive)
            .OrderBy(c => c.Priority)
            .Select(c => new ChallengeDto
            {
                ChallengeId = c.ChallengeId,
                ChallengeName = c.ChallengeName,
                Condition = c.Condition,
                RewardPoints = c.RewardPoints,
                Priority = c.Priority,
                IsActive = c.IsActive
            })
            .ToListAsync();

        return Ok(challenges);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChallenge(string id)
    {
        var challenge = await _context.Challenges
            .Where(c => c.ChallengeId == id)
            .Select(c => new ChallengeDto
            {
                ChallengeId = c.ChallengeId,
                ChallengeName = c.ChallengeName,
                Condition = c.Condition,
                RewardPoints = c.RewardPoints,
                Priority = c.Priority,
                IsActive = c.IsActive
            })
            .FirstOrDefaultAsync();

        if (challenge == null)
            return NotFound();

        return Ok(challenge);
    }
}