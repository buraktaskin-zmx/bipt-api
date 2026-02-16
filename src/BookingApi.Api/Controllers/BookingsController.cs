using System.Security.Claims;
using BookingApi.Application.DTOs;
using BookingApi.Application.DTOs.BookingDTOs;
using BookingApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        try
        {
            var result = await _bookingService.CreateAsync(GetUserId(), request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var result = await _bookingService.GetMyBookingsAsync(GetUserId());
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var success = await _bookingService.CancelAsync(GetUserId(), id);
        return success ? Ok(new { message = "İptal edildi." }) : NotFound();
    }
}