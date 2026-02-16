using BookingApi.Application.DTOs;
using BookingApi.Application.DTOs.AuthDTOs;
using BookingApi.Application.Services;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly AppDbContext _context;

    public LeaderboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaderboardDto>> GetLeaderboardAsync()
    {
        // 1. Ledger'dan toplam puanları hesapla
        var userPoints = await _context.PointsLedgers
            .GroupBy(pl => pl.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalPoints = g.Sum(pl => pl.PointsDelta)
            })
            .ToListAsync();

        // 2. Tüm kullanıcıları çek
        var allUsers = await _context.Users
            .Select(u => new
            {
                u.UserId,
                u.Name
            })
            .ToListAsync();

        // 3. Memory'de birleştir ve sırala
        var leaderboard = allUsers
            .Select(u => new
            {
                u.UserId,
                u.Name,
                TotalPoints = userPoints.FirstOrDefault(up => up.UserId == u.UserId)?.TotalPoints ?? 0
            })
            .OrderByDescending(x => x.TotalPoints)
            .ThenBy(x => x.UserId)
            .ToList();

        // 4. Rank ekle ve DTO'ya dönüştür
        var result = leaderboard.Select((item, index) => new LeaderboardDto
        {
            Rank = index + 1,
            UserId = item.UserId,
            Name = item.Name,
            TotalPoints = item.TotalPoints
        }).ToList();

        return result;
    }
}