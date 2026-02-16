using BookingApi.Application.DTOs;
using BookingApi.Application.Services;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Services;

public class MetricsService : IMetricsService
{
    private readonly AppDbContext _context;

    public MetricsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserStateDto?> GetUserMetricsAsync(string userId, DateTime asOfDate)
    {
        // UTC'ye çevir
        var asOfDateUtc = DateTime.SpecifyKind(asOfDate.Date, DateTimeKind.Utc);

        var userState = await _context.UserStates
            .Where(us => us.UserId == userId && us.AsOfDate.Date == asOfDateUtc.Date)
            .FirstOrDefaultAsync();

        if (userState == null)
            return null;

        var user = await _context.Users.FindAsync(userId);

        return new UserStateDto
        {
            UserId = userState.UserId,
            UserName = user?.Name ?? "",
            AsOfDate = userState.AsOfDate,
            MessagesToday = userState.MessagesToday,
            ReactionsToday = userState.ReactionsToday,
            UniqueGroupsToday = userState.UniqueGroupsToday,
            Messages7d = userState.Messages7d,
            Reactions7d = userState.Reactions7d
        };
    }

    public async Task<List<UserStateDto>> CalculateMetricsAsync(DateTime asOfDate)
    {
        // UTC'ye çevir
        var asOfDateUtc = DateTime.SpecifyKind(asOfDate.Date, DateTimeKind.Utc);

        var users = await _context.Users.ToListAsync();
        var result = new List<UserStateDto>();

        foreach (var user in users)
        {
            var metrics = await CalculateUserMetrics(user.UserId, asOfDateUtc);
            result.Add(metrics);
        }

        return result;
    }

    private async Task<UserStateDto> CalculateUserMetrics(string userId, DateTime asOfDate)
    {
        // asOfDate zaten UTC olduğunu varsayıyoruz (CalculateMetricsAsync'den geliyor)
        var sevenDaysAgo = asOfDate.AddDays(-6).Date;

        // Bugünkü aktivite
        var todayActivity = await _context.ActivityEvents
            .Where(ae => ae.UserId == userId && ae.Date.Date == asOfDate.Date)
            .FirstOrDefaultAsync();

        // Son 7 günün aktiviteleri
        var last7DaysActivities = await _context.ActivityEvents
            .Where(ae => ae.UserId == userId
                      && ae.Date.Date >= sevenDaysAgo
                      && ae.Date.Date <= asOfDate.Date)
            .ToListAsync();

        var user = await _context.Users.FindAsync(userId);

        var state = new UserStateDto
        {
            UserId = userId,
            UserName = user?.Name ?? "",
            AsOfDate = asOfDate,
            MessagesToday = todayActivity?.Messages ?? 0,
            ReactionsToday = todayActivity?.Reactions ?? 0,
            UniqueGroupsToday = todayActivity?.UniqueGroups ?? 0,
            Messages7d = last7DaysActivities.Sum(a => a.Messages),
            Reactions7d = last7DaysActivities.Sum(a => a.Reactions)
        };

        // Database'e kaydet veya güncelle
        var existingState = await _context.UserStates
            .FirstOrDefaultAsync(us => us.UserId == userId && us.AsOfDate.Date == asOfDate.Date);

        if (existingState != null)
        {
            existingState.MessagesToday = state.MessagesToday;
            existingState.ReactionsToday = state.ReactionsToday;
            existingState.UniqueGroupsToday = state.UniqueGroupsToday;
            existingState.Messages7d = state.Messages7d;
            existingState.Reactions7d = state.Reactions7d;
        }
        else
        {
            _context.UserStates.Add(new UserState
            {
                UserId = userId,
                AsOfDate = asOfDate, // Zaten UTC
                MessagesToday = state.MessagesToday,
                ReactionsToday = state.ReactionsToday,
                UniqueGroupsToday = state.UniqueGroupsToday,
                Messages7d = state.Messages7d,
                Reactions7d = state.Reactions7d
            });
        }

        await _context.SaveChangesAsync();

        return state;
    }
}