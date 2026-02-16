using BookingApi.Application.DTOs;
using BookingApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LedgerController : ControllerBase
{
    private readonly AppDbContext _context;

    public LedgerController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kullanıcının tüm puan hareketlerini getirir (en yeniden eskiye)
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserLedger(string userId)
    {
        // Kullanıcı var mı kontrol et
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        // Ledger kayıtlarını çek
        var ledgerEntries = await _context.PointsLedgers
            .Where(pl => pl.UserId == userId)
            .OrderByDescending(pl => pl.LedgerId) // En yeni önce
            .Select(pl => new PointsLedgerDto
            {
                LedgerId = pl.LedgerId,
                UserId = pl.UserId,
                PointsDelta = pl.PointsDelta,
                Source = pl.Source,
                SourceRef = pl.SourceRef
            })
            .ToListAsync();

        // Challenge referanslarını zenginleştir (SourceRef'ten challenge adını bul)
        foreach (var entry in ledgerEntries)
        {
            if (entry.Source == "CHALLENGE" && !string.IsNullOrEmpty(entry.SourceRef))
            {
                var prefix = entry.SourceRef.Split('-', 2)[0]; // <-- sorgu DIŞINDA

                var award = await _context.ChallengeAwards
                    .Include(ca => ca.Challenge)
                    .FirstOrDefaultAsync(ca => ca.IsSelected && ca.AwardId.StartsWith(prefix));

                if (award != null)
                    entry.Description = award.Challenge?.ChallengeName ?? "Challenge";
            }
        }

        return Ok(new
        {
            userId = userId,
            totalPoints = ledgerEntries.Sum(l => l.PointsDelta),
            transactions = ledgerEntries
        });
    }

    /// <summary>
    /// Kullanıcının toplam puanını getirir (özet)
    /// </summary>
    [HttpGet("{userId}/total")]
    public async Task<IActionResult> GetUserTotalPoints(string userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        var totalPoints = await _context.PointsLedgers
            .Where(pl => pl.UserId == userId)
            .SumAsync(pl => pl.PointsDelta);

        var transactionCount = await _context.PointsLedgers
            .Where(pl => pl.UserId == userId)
            .CountAsync();

        return Ok(new
        {
            userId = userId,
            totalPoints = totalPoints,
            transactionCount = transactionCount
        });
    }

    /// <summary>
    /// Tüm kullanıcıların puan istatistiklerini getirir (Admin için)
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetLedgerStatistics()
    {
        var statistics = await _context.PointsLedgers
            .GroupBy(pl => pl.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalPoints = g.Sum(pl => pl.PointsDelta),
                TransactionCount = g.Count(),
                AveragePointsPerTransaction = g.Average(pl => pl.PointsDelta),
                HighestSingleGain = g.Max(pl => pl.PointsDelta)
            })
            .OrderByDescending(s => s.TotalPoints)
            .ToListAsync();

        // User bilgilerini ekle
        var enrichedStats = new List<object>();
        foreach (var stat in statistics)
        {
            var user = await _context.Users.FindAsync(stat.UserId);
            enrichedStats.Add(new
            {
                userId = stat.UserId,
                userName = user?.Name ?? "Unknown",
                totalPoints = stat.TotalPoints,
                transactionCount = stat.TransactionCount,
                averagePointsPerTransaction = Math.Round(stat.AveragePointsPerTransaction, 2),
                highestSingleGain = stat.HighestSingleGain
            });
        }

        return Ok(enrichedStats);
    }

    /// <summary>
    /// Kaynak tipine göre puan dağılımını getirir
    /// </summary>
    [HttpGet("{userId}/breakdown")]
    public async Task<IActionResult> GetPointsBreakdown(string userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        var breakdown = await _context.PointsLedgers
            .Where(pl => pl.UserId == userId)
            .GroupBy(pl => pl.Source)
            .Select(g => new
            {
                Source = g.Key,
                TotalPoints = g.Sum(pl => pl.PointsDelta),
                Count = g.Count()
            })
            .ToListAsync();

        var totalPoints = breakdown.Sum(b => b.TotalPoints);

        return Ok(new
        {
            userId = userId,
            totalPoints = totalPoints,
            breakdown = breakdown
        });
    }
}
