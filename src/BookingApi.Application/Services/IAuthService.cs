using BookingApi.Application.DTOs.AuthDTOs;

namespace BookingApi.Application.Services
{
    public interface IAuthService
    {
        public Task<AuthResponse> RegisterAsync(RegisterRequest request);
        public Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
