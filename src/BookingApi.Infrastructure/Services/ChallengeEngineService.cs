using BookingApi.Application.DTOs;
using BookingApi.Application.Services;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Services;

public class ChallengeEngineService : IChallengeEngineService
{
    private readonly AppDbContext _context;

    public ChallengeEngineService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChallengeEvaluationResultDto>> EvaluateAllUsersAsync(DateTime asOfDate)
    {
        var asOfDateUtc = DateTime.SpecifyKind(asOfDate.Date, DateTimeKind.Utc);

        var users = await _context.Users.ToListAsync();
        var challenges = await _context.Challenges.Where(c => c.IsActive).ToListAsync();
        var results = new List<ChallengeEvaluationResultDto>();

        foreach (var user in users)
        {
            var result = await EvaluateUserChallenges(user, challenges, asOfDateUtc);
            results.Add(result);
        }

        return results;
    }

    private async Task<ChallengeEvaluationResultDto> EvaluateUserChallenges(
        User user,
        List<Challenge> challenges,
        DateTime asOfDate)
    {
        // 1. Kullanıcının metriklerini al
        var userState = await _context.UserStates
            .FirstOrDefaultAsync(us => us.UserId == user.UserId && us.AsOfDate.Date == asOfDate.Date);

        if (userState == null)
        {
            return new ChallengeEvaluationResultDto
            {
                UserId = user.UserId,
                UserName = user.Name,
                TriggeredChallenges = new(),
                SelectedChallenge = null,
                PointsAwarded = 0
            };
        }

        // 2. Hangi challenge'lar tetiklendi?
        var triggeredChallenges = new List<Challenge>();

        foreach (var challenge in challenges)
        {
            if (EvaluateCondition(challenge.Condition, userState))
            {
                triggeredChallenges.Add(challenge);
            }
        }

        if (!triggeredChallenges.Any())
        {
            return new ChallengeEvaluationResultDto
            {
                UserId = user.UserId,
                UserName = user.Name,
                TriggeredChallenges = new(),
                SelectedChallenge = null,
                PointsAwarded = 0
            };
        }

        // 3. Priority'ye göre sırala (en küçük priority = en önemli)
        var selectedChallenge = triggeredChallenges.OrderBy(c => c.Priority).First();
        var suppressed = triggeredChallenges.Where(c => c.ChallengeId != selectedChallenge.ChallengeId).ToList();

        // 4. ChallengeAwards'a kaydet
        var awardId = $"A-{Guid.NewGuid().ToString().Substring(0, 8)}";

        // Seçilen challenge
        _context.ChallengeAwards.Add(new ChallengeAward
        {
            AwardId = $"{awardId}-1",
            UserId = user.UserId,
            ChallengeId = selectedChallenge.ChallengeId,
            AsOfDate = asOfDate,
            IsSelected = true
        });

        // Suppressed challenge'lar
        int suppressedIndex = 2;
        foreach (var suppressedChallenge in suppressed)
        {
            _context.ChallengeAwards.Add(new ChallengeAward
            {
                AwardId = $"{awardId}-{suppressedIndex++}",
                UserId = user.UserId,
                ChallengeId = suppressedChallenge.ChallengeId,
                AsOfDate = asOfDate,
                IsSelected = false
            });
        }

        // 5. PointsLedger'a puan ekle
        var ledgerId = $"L-{Guid.NewGuid().ToString().Substring(0, 8)}";
        _context.PointsLedgers.Add(new PointsLedger
        {
            LedgerId = ledgerId,
            UserId = user.UserId,
            PointsDelta = selectedChallenge.RewardPoints,
            Source = "CHALLENGE",
            SourceRef = awardId
        });

        // 6. Bildirim oluştur
        var notificationId = $"N-{Guid.NewGuid().ToString().Substring(0, 8)}";
        _context.Notifications.Add(new Notification
        {
            NotificationId = notificationId,
            UserId = user.UserId,
            Message = $"🎯 Tebrikler! '{selectedChallenge.ChallengeName}' görevini tamamladın. +{selectedChallenge.RewardPoints} puan!",
            Type = "CHALLENGE",
            SourceRef = awardId,
            IsRead = false
        });

        await _context.SaveChangesAsync();

        return new ChallengeEvaluationResultDto
        {
            UserId = user.UserId,
            UserName = user.Name,
            TriggeredChallenges = triggeredChallenges.Select(c => c.ChallengeId).ToList(),
            SelectedChallenge = selectedChallenge.ChallengeId,
            PointsAwarded = selectedChallenge.RewardPoints,
            SuppressedChallenges = suppressed.Select(c => c.ChallengeId).ToList()
        };
    }

    private bool EvaluateCondition(string condition, UserState userState)
    {
        // Basit condition parser
        // Örnek: "messages_today >= 20"

        var parts = condition.Split(new[] { ">=", "<=", ">", "<", "==" }, StringSplitOptions.None);
        if (parts.Length != 2) return false;

        var metric = parts[0].Trim();
        if (!int.TryParse(parts[1].Trim(), out int threshold)) return false;

        int actualValue = metric switch
        {
            "messages_today" => userState.MessagesToday,
            "reactions_today" => userState.ReactionsToday,
            "unique_groups_today" => userState.UniqueGroupsToday,
            "messages_7d" => userState.Messages7d,
            "reactions_7d" => userState.Reactions7d,
            _ => 0
        };

        // Operator kontrolü
        if (condition.Contains(">=")) return actualValue >= threshold;
        if (condition.Contains("<=")) return actualValue <= threshold;
        if (condition.Contains(">")) return actualValue > threshold;
        if (condition.Contains("<")) return actualValue < threshold;
        if (condition.Contains("==")) return actualValue == threshold;

        return false;
    }

    public async Task<List<ChallengeAwardDto>> GetUserChallengeAwardsAsync(string userId, DateTime asOfDate)
    {
        var asOfDateUtc = DateTime.SpecifyKind(asOfDate.Date, DateTimeKind.Utc);

        var awards = await _context.ChallengeAwards
            .Where(ca => ca.UserId == userId && ca.AsOfDate.Date == asOfDateUtc.Date)
            .Include(ca => ca.Challenge)
            .Select(ca => new ChallengeAwardDto
            {
                AwardId = ca.AwardId,
                UserId = ca.UserId,
                ChallengeId = ca.ChallengeId,
                ChallengeName = ca.Challenge!.ChallengeName,
                RewardPoints = ca.Challenge.RewardPoints,
                IsSelected = ca.IsSelected,
                AsOfDate = ca.AsOfDate
            })
            .ToListAsync();

        return awards;
    }
}