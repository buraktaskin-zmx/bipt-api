using BookingApi.Application.DTOs;
using BookingApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tüm BiP gruplarını listeler
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _context.Groups
            .Select(g => new GroupDto
            {
                GroupId = g.GroupId,
                GroupName = g.GroupName
            })
            .OrderBy(g => g.GroupId)
            .ToListAsync();

        return Ok(groups);
    }

    /// <summary>
    /// Belirli bir grubun detaylarını getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup(string id)
    {
        var group = await _context.Groups
            .Where(g => g.GroupId == id)
            .Select(g => new GroupDto
            {
                GroupId = g.GroupId,
                GroupName = g.GroupName
            })
            .FirstOrDefaultAsync();

        if (group == null)
            return NotFound(new { message = $"Grup bulunamadı: {id}" });

        return Ok(group);
    }
}