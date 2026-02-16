using BookingApi.Application.Services;
using BookingApi.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;

    public MetricsController(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserMetrics(string userId, [FromQuery] DateTime asOfDate)
    {
        var metrics = await _metricsService.GetUserMetricsAsync(userId, asOfDate);

        if (metrics == null)
            return NotFound();

        return Ok(metrics);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateMetrics([FromQuery] DateTime asOfDate)
    {
        var metrics = await _metricsService.CalculateMetricsAsync(asOfDate);
        return Ok(metrics);
    }
}