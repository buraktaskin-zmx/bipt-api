using BookingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(255);
            e.Property(u => u.DisplayName).HasMaxLength(255);
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Service).HasMaxLength(50);
            e.Property(b => b.Status).HasMaxLength(20);
            e.HasOne(b => b.User).WithMany().HasForeignKey(b => b.UserId);
        });
    }
}