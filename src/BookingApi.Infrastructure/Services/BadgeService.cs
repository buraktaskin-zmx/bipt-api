using BookingApi.Application.DTOs;
using BookingApi.Application.Services;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Services;

public class BadgeService : IBadgeService
{
    private readonly AppDbContext _context;

    public BadgeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BadgeEvaluationResultDto>> EvaluateAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        var results = new List<BadgeEvaluationResultDto>();

        foreach (var user in users)
        {
            var result = await EvaluateUserAsync(user.UserId);
            results.Add(result);
        }

        return results;
    }

    public async Task<BadgeEvaluationResultDto> EvaluateUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return new BadgeEvaluationResultDto
            {
                UserId = userId,
                UserName = "Unknown"
            };
        }

        // 1. Kullanıcının toplam puanını hesapla
        var totalPoints = await _context.PointsLedgers
            .Where(pl => pl.UserId == userId)
            .SumAsync(pl => pl.PointsDelta);

        // 2. Tüm badge'leri al
        var allBadges = await _context.Badges
            .OrderBy(b => b.Level)
            .ToListAsync();

        // 3. Kullanıcının mevcut badge'lerini al
        var existingBadgeIds = await _context.BadgeAwards
            .Where(ba => ba.UserId == userId)
            .Select(ba => ba.BadgeId)
            .ToListAsync();

        // 4. Hangi badge'leri kazanması gerekiyor?
        var earnedBadges = new List<Badge>();
        foreach (var badge in allBadges)
        {
            if (EvaluateBadgeCondition(badge.Condition, totalPoints))
            {
                earnedBadges.Add(badge);
            }
        }

        // 5. Yeni kazanılan badge'leri tespit et
        var newBadges = new List<BadgeAwardResultDto>();
        foreach (var badge in earnedBadges)
        {
            if (!existingBadgeIds.Contains(badge.BadgeId))
            {
                // Yeni badge! Database'e ekle
                var awardId = $"BA-{Guid.NewGuid().ToString().Substring(0, 8)}";

                _context.BadgeAwards.Add(new BadgeAward
                {
                    AwardId = awardId,
                    UserId = userId,
                    BadgeId = badge.BadgeId
                });

                // Bildirim oluştur
                var notificationId = $"N-{Guid.NewGuid().ToString().Substring(0, 8)}";
                _context.Notifications.Add(new Notification
                {
                    NotificationId = notificationId,
                    UserId = userId,
                    Message = $"🎖️ Tebrikler! '{badge.BadgeName}' rozetini kazandın!",
                    Type = "BADGE",
                    SourceRef = awardId,
                    IsRead = false
                });

                newBadges.Add(new BadgeAwardResultDto
                {
                    BadgeId = badge.BadgeId,
                    BadgeName = badge.BadgeName,
                    Level = badge.Level,
                    IsNew = true
                });
            }
        }

        await _context.SaveChangesAsync();

        return new BadgeEvaluationResultDto
        {
            UserId = userId,
            UserName = user.Name,
            TotalPoints = totalPoints,
            NewBadges = newBadges,
            ExistingBadges = existingBadgeIds
        };
    }

    private bool EvaluateBadgeCondition(string condition, int totalPoints)
    {
        // Basit condition parser
        // Örnek: "total_points >= 200"

        var parts = condition.Split(new[] { ">=", "<=", ">", "<", "==" }, StringSplitOptions.None);
        if (parts.Length != 2) return false;

        var metric = parts[0].Trim();
        if (!int.TryParse(parts[1].Trim(), out int threshold)) return false;

        // Badge condition'ları sadece total_points kontrolü yapar
        if (metric != "total_points") return false;

        // Operator kontrolü
        if (condition.Contains(">=")) return totalPoints >= threshold;
        if (condition.Contains("<=")) return totalPoints <= threshold;
        if (condition.Contains(">")) return totalPoints > threshold;
        if (condition.Contains("<")) return totalPoints < threshold;
        if (condition.Contains("==")) return totalPoints == threshold;

        return false;
    }
}