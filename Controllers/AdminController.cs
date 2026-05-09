using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Services;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminController(AppDbContext context, IClerkService clerkService) : ControllerBase
{
    private static readonly string[] ValidRoles = ["admin", "manager", "gm", "player", "viewer"];

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await context.Users
            .OrderBy(u => u.CreatedAt)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Role = u.Role,
                SceneCount = context.Scenes.Count(sc => sc.Story.UserId == u.Id),
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
        return Ok(users);
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request)
    {
        if (!ValidRoles.Contains(request.Role))
            return BadRequest("Invalid role.");

        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Role = request.Role;
        await context.SaveChangesAsync();
        await clerkService.UpdateUserRoleAsync(id, request.Role);

        return NoContent();
    }
}

public class AdminUserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Role { get; set; }
    public int SceneCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateRoleRequest { public string Role { get; set; } = null!; }
