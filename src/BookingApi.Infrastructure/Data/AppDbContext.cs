using BookingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<PointsLedger> PointsLedgers { get; set; }
    public DbSet<Badge> Badges { get; set; } // ← YENİ
    public DbSet<BadgeAward> BadgeAwards { get; set; } // ← YENİ
    public DbSet<Challenge> Challenges { get; set; } // ← YENİ
    public DbSet<ActivityEvent> ActivityEvents { get; set; }
    public DbSet<UserState> UserStates { get; set; }
    public DbSet<ChallengeAward> ChallengeAwards { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.City).HasMaxLength(100);
        });
        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId);
            entity.Property(e => e.ChallengeId).HasMaxLength(10);
            entity.Property(e => e.ChallengeName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Condition).HasMaxLength(200).IsRequired();
        });
        modelBuilder.Entity<ActivityEvent>(entity =>
        {
            entity.HasKey(e => e.EventId);
            entity.Property(e => e.EventId).HasMaxLength(20);
            entity.Property(e => e.UserId).HasMaxLength(10).IsRequired();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<ChallengeAward>(entity =>
        {
            entity.HasKey(e => e.AwardId);
            entity.Property(e => e.AwardId).HasMaxLength(20);
            entity.Property(e => e.UserId).HasMaxLength(10).IsRequired();
            entity.Property(e => e.ChallengeId).HasMaxLength(10).IsRequired();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Challenge).WithMany().HasForeignKey(e => e.ChallengeId);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.NotificationId).HasMaxLength(20);
            entity.Property(e => e.UserId).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Message).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(20).IsRequired();
            entity.Property(e => e.SourceRef).HasMaxLength(50);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });

        // UserState - Composite Key
        modelBuilder.Entity<UserState>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.AsOfDate });
            entity.Property(e => e.UserId).HasMaxLength(10);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });
        // Group configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId);
            entity.Property(e => e.GroupId).HasMaxLength(10);
            entity.Property(e => e.GroupName).HasMaxLength(100).IsRequired();
        });


        // PointsLedger configuration
        modelBuilder.Entity<PointsLedger>(entity =>
        {
            entity.HasKey(e => e.LedgerId);
            entity.Property(e => e.LedgerId).HasMaxLength(20);
            entity.Property(e => e.UserId).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Source).HasMaxLength(50).IsRequired();
            entity.Property(e => e.SourceRef).HasMaxLength(50);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });

        // Badge configuration ← YENİ
        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.BadgeId);
            entity.Property(e => e.BadgeId).HasMaxLength(10);
            entity.Property(e => e.BadgeName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Condition).HasMaxLength(200).IsRequired();
        });

        // BadgeAward configuration ← YENİ
        modelBuilder.Entity<BadgeAward>(entity =>
        {
            entity.HasKey(e => e.AwardId);
            entity.Property(e => e.AwardId).HasMaxLength(20);
            entity.Property(e => e.UserId).HasMaxLength(10).IsRequired();
            entity.Property(e => e.BadgeId).HasMaxLength(10).IsRequired();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Badge).WithMany().HasForeignKey(e => e.BadgeId);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Users
        modelBuilder.Entity<User>().HasData(
            new User { UserId = "U1", Name = "Ayşe", City = "Istanbul" },
            new User { UserId = "U2", Name = "Ali", City = "Ankara" },
            new User { UserId = "U3", Name = "Deniz", City = "Izmir" },
            new User { UserId = "U4", Name = "Mert", City = "Bursa" },
            new User { UserId = "U5", Name = "Ece", City = "Antalya" }
        );

        // Groups
        modelBuilder.Entity<Group>().HasData(
            new Group { GroupId = "G1", GroupName = "Tech Sohbet" },
            new Group { GroupId = "G2", GroupName = "Futbol Muhabbet" },
            new Group { GroupId = "G3", GroupName = "Oyun Ekibi" }
        );

        // PointsLedger
        modelBuilder.Entity<PointsLedger>().HasData(
            new PointsLedger
            {
                LedgerId = "L-200",
                UserId = "U1",
                PointsDelta = 200,
                Source = "CHALLENGE",
                SourceRef = "A-100"
            },
            new PointsLedger
            {
                LedgerId = "L-201",
                UserId = "U2",
                PointsDelta = 200,
                Source = "CHALLENGE",
                SourceRef = "A-101"
            },
            new PointsLedger
            {
                LedgerId = "L-202",
                UserId = "U3",
                PointsDelta = 200,
                Source = "CHALLENGE",
                SourceRef = "A-102"
            },
            new PointsLedger
            {
                LedgerId = "L-203",
                UserId = "U4",
                PointsDelta = 200,
                Source = "CHALLENGE",
                SourceRef = "A-103"
            },
            new PointsLedger
            {
                LedgerId = "L-204",
                UserId = "U5",
                PointsDelta = 50,
                Source = "CHALLENGE",
                SourceRef = "A-104"
            }
        );

        // Badges ← YENİ
        modelBuilder.Entity<Badge>().HasData(
            new Badge
            {
                BadgeId = "B1",
                BadgeName = "🥉 Bronz Sosyal",
                Condition = "total_points >= 200",
                Level = 1
            },
            new Badge
            {
                BadgeId = "B2",
                BadgeName = "🥈 Gümüş Sosyal",
                Condition = "total_points >= 600",
                Level = 2
            },
            new Badge
            {
                BadgeId = "B3",
                BadgeName = "🥇 Altın Sosyal",
                Condition = "total_points >= 1000",
                Level = 3
            }
        );

        // BadgeAwards ← YENİ (U1-U4 kazandı, U5 kazanmadı)
        modelBuilder.Entity<BadgeAward>().HasData(
            new BadgeAward { AwardId = "BA-1", UserId = "U1", BadgeId = "B1" },
            new BadgeAward { AwardId = "BA-2", UserId = "U2", BadgeId = "B1" },
            new BadgeAward { AwardId = "BA-3", UserId = "U3", BadgeId = "B1" },
            new BadgeAward { AwardId = "BA-4", UserId = "U4", BadgeId = "B1" }
        );

        modelBuilder.Entity<Challenge>().HasData(
    new Challenge
    {
        ChallengeId = "C-01",
        ChallengeName = "Günlük Mesajcı",
        Condition = "messages_today >= 20",
        RewardPoints = 50,
        Priority = 4,
        IsActive = true
    },
    new Challenge
    {
        ChallengeId = "C-02",
        ChallengeName = "Etkileşim Ustası",
        Condition = "reactions_today >= 15",
        RewardPoints = 80,
        Priority = 3,
        IsActive = true
    },
    new Challenge
    {
        ChallengeId = "C-03",
        ChallengeName = "Grup Lideri",
        Condition = "unique_groups_today >= 3",
        RewardPoints = 120,
        Priority = 2,
        IsActive = true
    },
    new Challenge
    {
        ChallengeId = "C-04",
        ChallengeName = "Haftalık Aktif",
        Condition = "messages_7d >= 150",
        RewardPoints = 200,
        Priority = 1,
        IsActive = true
    }
);


        // ActivityEvents - UTC ile
        modelBuilder.Entity<ActivityEvent>().HasData(
            new ActivityEvent { EventId = "E-1", UserId = "U1", Date = DateTime.SpecifyKind(new DateTime(2026, 3, 6), DateTimeKind.Utc), Messages = 13, Reactions = 7, UniqueGroups = 1 },
            new ActivityEvent { EventId = "E-2", UserId = "U1", Date = DateTime.SpecifyKind(new DateTime(2026, 3, 7), DateTimeKind.Utc), Messages = 33, Reactions = 5, UniqueGroups = 3 },
            new ActivityEvent { EventId = "E-3", UserId = "U1", Date = DateTime.SpecifyKind(new DateTime(2026, 3, 8), DateTimeKind.Utc), Messages = 12, Reactions = 23, UniqueGroups = 3 },
            new ActivityEvent { EventId = "E-4", UserId = "U1", Date = DateTime.SpecifyKind(new DateTime(2026, 3, 9), DateTimeKind.Utc), Messages = 27, Reactions = 25, UniqueGroups = 1 },
            new ActivityEvent { EventId = "E-5", UserId = "U1", Date = DateTime.SpecifyKind(new DateTime(2026, 3, 10), DateTimeKind.Utc), Messages = 19, Reactions = 8, UniqueGroups = 1 },
            new ActivityEvent { EventId = "E-6", UserId = "U1", Date = DateTime.SpecifyKind(new DateTime(2026, 3, 11), DateTimeKind.Utc), Messages = 25, Reactions = 19, UniqueGroups = 1 },
            new ActivityEvent { EventId = "E-7", UserId = "U1", Date = DateTime.SpecifyKind(new DateTime(2026, 3, 12), DateTimeKind.Utc), Messages = 400, Reactions = 21, UniqueGroups = 3 }
        );



        var asOfDate = DateTime.SpecifyKind(new DateTime(2026, 3, 12), DateTimeKind.Utc);

        modelBuilder.Entity<UserState>().HasData(
            new UserState
            {
                UserId = "U1",
                AsOfDate = asOfDate,
                MessagesToday = 400,
                ReactionsToday = 21,
                UniqueGroupsToday = 3,
                Messages7d = 169,
                Reactions7d = 108
            },
            new UserState
            {
                UserId = "U2",
                AsOfDate = asOfDate,
                MessagesToday = 26,
                ReactionsToday = 8,
                UniqueGroupsToday = 1,
                Messages7d = 150,
                Reactions7d = 109
            },
            new UserState
            {
                UserId = "U3",
                AsOfDate = asOfDate,
                MessagesToday = 23,
                ReactionsToday = 16,
                UniqueGroupsToday = 2,
                Messages7d = 185,
                Reactions7d = 88
            },
            new UserState
            {
                UserId = "U4",
                AsOfDate = asOfDate,
                MessagesToday = 38,
                ReactionsToday = 13,
                UniqueGroupsToday = 1,
                Messages7d = 218,
                Reactions7d = 129
            },
            new UserState
            {
                UserId = "U5",
                AsOfDate = asOfDate,
                MessagesToday = 29,
                ReactionsToday = 11,
                UniqueGroupsToday = 1,
                Messages7d = 104,
                Reactions7d = 70
            }
        );
    }
}