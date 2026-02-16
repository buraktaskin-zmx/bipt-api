//using BookingApi.Application.DTOs;
//using BookingApi.Application.DTOs.BookingDTOs;
//using BookingApi.Application.Services;
//using BookingApi.Domain.Entities;
//using BookingApi.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;

//namespace BookingApi.Infrastructure.Services;

//public class BookingService : IBookingService
//{
//    private readonly AppDbContext _db;

//    public BookingService(AppDbContext db)
//    {
//        _db = db;
//    }

//    public async Task<BookingResponse> CreateAsync(Guid userId, CreateBookingRequest request)
//    {
//        // Çakışma kontrolü: aynı serviste aynı zaman diliminde başka booking var mı?
//        var hasConflict = await _db.Bookings.AnyAsync(b =>
//            b.Service == request.Service &&
//            b.Status == "confirmed" &&
//            b.StartTime < request.EndTime &&
//            b.EndTime > request.StartTime);

//        if (hasConflict)
//            throw new Exception("Bu zaman diliminde seçilen servis için zaten bir rezervasyon var.");

//        var booking = new Booking
//        {
//            Id = Guid.NewGuid(),
//            UserId = userId,
//            Service = request.Service,
//            StartTime = request.StartTime,
//            EndTime = request.EndTime
//        };

//        _db.Bookings.Add(booking);
//        await _db.SaveChangesAsync();

//        return ToResponse(booking);
//    }

//    public async Task<List<BookingResponse>> GetMyBookingsAsync(Guid userId)
//    {
//        return await _db.Bookings
//            .Where(b => b.UserId == userId)
//            .OrderByDescending(b => b.StartTime)
//            .Select(b => ToResponse(b))
//            .ToListAsync();
//    }

//    public async Task<bool> CancelAsync(Guid userId, Guid bookingId)
//    {
//        var booking = await _db.Bookings
//            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

//        if (booking is null) return false;

//        booking.Status = "cancelled";
//        await _db.SaveChangesAsync();
//        return true;
//    }

//    private static BookingResponse ToResponse(Booking b) => new(
//        b.Id, b.Service, b.StartTime, b.EndTime, b.Status, b.CreatedAt
//    );
//}