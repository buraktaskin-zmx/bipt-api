using System;
using System.Collections.Generic;
using System.Text;
using BookingApi.Application.DTOs;
using BookingApi.Application.Services;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Services
{

    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto?> GetUserDashboardAsync(string userId)
        {
            // 1. User bilgisi
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    City = u.City
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            // 2. Toplam puan
            var totalPoints = await _context.PointsLedgers
                .Where(pl => pl.UserId == userId)
                .SumAsync(pl => pl.PointsDelta);

            // 3. Leaderboard sıralaması
            var allUserPoints = await _context.PointsLedgers
                .GroupBy(pl => pl.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalPoints = g.Sum(pl => pl.PointsDelta)
                })
                .ToListAsync();

            var leaderboard = allUserPoints
                .OrderByDescending(x => x.TotalPoints)
                .ThenBy(x => x.UserId)
                .ToList();

            var rank = leaderboard.FindIndex(x => x.UserId == userId) + 1;

            // 4. Rozetler
            var badges = await _context.BadgeAwards
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

            // 5. Son aktiviteler
            var recentActivity = await _context.PointsLedgers
                .Where(pl => pl.UserId == userId)
                .OrderByDescending(pl => pl.LedgerId)
                .Take(5)
                .Select(pl => new PointsActivityDto
                {
                    LedgerId = pl.LedgerId,
                    PointsDelta = pl.PointsDelta,
                    Source = pl.Source,
                    SourceRef = pl.SourceRef
                })
                .ToListAsync();

            return new DashboardDto
            {
                User = user,
                TotalPoints = totalPoints,
                LeaderboardRank = rank,
                Badges = badges,
                RecentActivity = recentActivity
            };
        }
    }
}
