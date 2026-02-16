using BookingApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDashboard(string userId)
    {
        var dashboard = await _dashboardService.GetUserDashboardAsync(userId);

        if (dashboard == null)
            return NotFound();

        return Ok(dashboard);
    }
}
