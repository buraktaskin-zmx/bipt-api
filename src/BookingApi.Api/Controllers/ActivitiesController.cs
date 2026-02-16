using BookingApi.Application.DTOs;
using BookingApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ActivitiesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kullanıcının tüm aktivite geçmişini getirir
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserActivities(string userId)
    {
        // Kullanıcı var mı kontrol et
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        // Aktiviteleri çek (tarihe göre azalan sırada)
        var activities = await _context.ActivityEvents
            .Where(ae => ae.UserId == userId)
            .OrderByDescending(ae => ae.Date)
            .Select(ae => new ActivityEventDto
            {
                EventId = ae.EventId,
                UserId = ae.UserId,
                Date = ae.Date,
                Messages = ae.Messages,
                Reactions = ae.Reactions,
                UniqueGroups = ae.UniqueGroups
            })
            .ToListAsync();

        return Ok(activities);
    }

    /// <summary>
    /// Belirli bir tarih aralığındaki aktiviteleri getirir
    /// </summary>
    [HttpGet("{userId}/range")]
    public async Task<IActionResult> GetUserActivitiesByDateRange(
        string userId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        // UTC'ye çevir
        var startUtc = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);

        var activities = await _context.ActivityEvents
            .Where(ae => ae.UserId == userId
                      && ae.Date.Date >= startUtc.Date
                      && ae.Date.Date <= endUtc.Date)
            .OrderByDescending(ae => ae.Date)
            .Select(ae => new ActivityEventDto
            {
                EventId = ae.EventId,
                UserId = ae.UserId,
                Date = ae.Date,
                Messages = ae.Messages,
                Reactions = ae.Reactions,
                UniqueGroups = ae.UniqueGroups
            })
            .ToListAsync();

        return Ok(activities);
    }

    /// <summary>
    /// Tüm kullanıcıların son aktivitelerini özetler (Admin için)
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetActivitiesSummary()
    {
        var summary = await _context.ActivityEvents
            .GroupBy(ae => ae.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalEvents = g.Count(),
                TotalMessages = g.Sum(ae => ae.Messages),
                TotalReactions = g.Sum(ae => ae.Reactions),
                LastActivityDate = g.Max(ae => ae.Date),
                AverageMessagesPerDay = g.Average(ae => ae.Messages)
            })
            .ToListAsync();

        return Ok(summary);
    }
}