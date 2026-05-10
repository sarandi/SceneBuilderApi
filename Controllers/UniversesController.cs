using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/universes")]
[Authorize]
public class UniversesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    [HttpGet]
    public async Task<IActionResult> GetUniverses()
    {
        var userId = GetClerkUserId();
        var universes = await context.Universes
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.UpdatedAt)
            .Select(u => new UniverseDto
            {
                Id = u.Id,
                Name = u.Name,
                Description = u.Description,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
            })
            .ToListAsync();
        return Ok(universes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUniverse(int id)
    {
        var userId = GetClerkUserId();
        var universe = await context.Universes
            .Where(u => u.Id == id && u.UserId == userId)
            .Select(u => new UniverseDto
            {
                Id = u.Id,
                Name = u.Name,
                Description = u.Description,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
            })
            .FirstOrDefaultAsync();
        if (universe == null) return NotFound();
        return Ok(universe);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUniverse([FromBody] UniverseRequest request)
    {
        var userId = GetClerkUserId();
        var universe = new Universe
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        context.Universes.Add(universe);
        await context.SaveChangesAsync();
        return Ok(new UniverseDto { Id = universe.Id, Name = universe.Name, Description = universe.Description, CreatedAt = universe.CreatedAt, UpdatedAt = universe.UpdatedAt });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUniverse(int id, [FromBody] UniverseRequest request)
    {
        var userId = GetClerkUserId();
        var universe = await context.Universes.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
        if (universe == null) return NotFound();
        universe.Name = request.Name;
        universe.Description = request.Description;
        universe.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Ok(new UniverseDto { Id = universe.Id, Name = universe.Name, Description = universe.Description, CreatedAt = universe.CreatedAt, UpdatedAt = universe.UpdatedAt });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUniverse(int id)
    {
        var userId = GetClerkUserId();
        var universe = await context.Universes.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
        if (universe == null) return NotFound();
        context.Universes.Remove(universe);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public class UniverseRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UniverseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
