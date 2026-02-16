using BookingApi.Application.DTOs;
using BookingApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificationsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kullanıcının tüm bildirimlerini getirir (en yeniden eskiye)
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserNotifications(string userId)
    {
        // Kullanıcı var mı kontrol et
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.NotificationId) // En yeni önce
            .Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Message = n.Message,
                Type = n.Type,
                SourceRef = n.SourceRef,
                IsRead = n.IsRead
            })
            .ToListAsync();

        var unreadCount = notifications.Count(n => !n.IsRead);

        return Ok(new
        {
            userId = userId,
            totalCount = notifications.Count,
            unreadCount = unreadCount,
            notifications = notifications
        });
    }

    /// <summary>
    /// Sadece okunmamış bildirimleri getirir
    /// </summary>
    [HttpGet("{userId}/unread")]
    public async Task<IActionResult> GetUnreadNotifications(string userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.NotificationId)
            .Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Message = n.Message,
                Type = n.Type,
                SourceRef = n.SourceRef,
                IsRead = n.IsRead
            })
            .ToListAsync();

        return Ok(new
        {
            userId = userId,
            unreadCount = notifications.Count,
            notifications = notifications
        });
    }

    /// <summary>
    /// Belirli bir bildirimi okundu olarak işaretle
    /// </summary>
    [HttpPut("{notificationId}/mark-read")]
    public async Task<IActionResult> MarkAsRead(string notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

        if (notification == null)
            return NotFound(new { message = $"Bildirim bulunamadı: {notificationId}" });

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            notificationId = notificationId,
            message = "Bildirim okundu olarak işaretlendi"
        });
    }

    /// <summary>
    /// Kullanıcının tüm bildirimlerini okundu olarak işaretle
    /// </summary>
    [HttpPut("{userId}/mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead(string userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            userId = userId,
            markedCount = notifications.Count,
            message = $"{notifications.Count} bildirim okundu olarak işaretlendi"
        });
    }

    /// <summary>
    /// Bildirimi sil
    /// </summary>
    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(string notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

        if (notification == null)
            return NotFound(new { message = $"Bildirim bulunamadı: {notificationId}" });

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            notificationId = notificationId,
            message = "Bildirim silindi"
        });
    }

    /// <summary>
    /// Kullanıcının tüm bildirimlerini sil
    /// </summary>
    [HttpDelete("{userId}/clear-all")]
    public async Task<IActionResult> ClearAllNotifications(string userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .ToListAsync();

        _context.Notifications.RemoveRange(notifications);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            userId = userId,
            deletedCount = notifications.Count,
            message = $"{notifications.Count} bildirim silindi"
        });
    }

    /// <summary>
    /// Tipe göre bildirimleri filtrele (CHALLENGE veya BADGE)
    /// </summary>
    [HttpGet("{userId}/type/{type}")]
    public async Task<IActionResult> GetNotificationsByType(string userId, string type)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            return NotFound(new { message = $"Kullanıcı bulunamadı: {userId}" });

        // Type validation
        if (type != "CHALLENGE" && type != "BADGE")
            return BadRequest(new { message = "Geçersiz tip. 'CHALLENGE' veya 'BADGE' olmalı" });

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.Type == type)
            .OrderByDescending(n => n.NotificationId)
            .Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Message = n.Message,
                Type = n.Type,
                SourceRef = n.SourceRef,
                IsRead = n.IsRead
            })
            .ToListAsync();

        return Ok(new
        {
            userId = userId,
            type = type,
            count = notifications.Count,
            notifications = notifications
        });
    }

    /// <summary>
    /// Bildirim istatistikleri (Admin için)
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetNotificationStatistics()
    {
        var stats = await _context.Notifications
            .GroupBy(n => n.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalNotifications = g.Count(),
                UnreadNotifications = g.Count(n => !n.IsRead),
                ChallengeNotifications = g.Count(n => n.Type == "CHALLENGE"),
                BadgeNotifications = g.Count(n => n.Type == "BADGE")
            })
            .ToListAsync();

        // User bilgilerini ekle
        var enrichedStats = new List<object>();
        foreach (var stat in stats)
        {
            var user = await _context.Users.FindAsync(stat.UserId);
            enrichedStats.Add(new
            {
                userId = stat.UserId,
                userName = user?.Name ?? "Unknown",
                totalNotifications = stat.TotalNotifications,
                unreadNotifications = stat.UnreadNotifications,
                challengeNotifications = stat.ChallengeNotifications,
                badgeNotifications = stat.BadgeNotifications
            });
        }

        return Ok(enrichedStats);
    }
}