using BookingApi.Application.DTOs.BookingDTOs;

namespace BookingApi.Application.Services;

public interface IBookingService
{
    public Task<BookingResponse> CreateAsync(Guid userId, CreateBookingRequest request);
    public Task<List<BookingResponse>> GetMyBookingsAsync(Guid userId);
    public Task<bool> CancelAsync(Guid userId, Guid bookingId);
}