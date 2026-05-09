using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/scenes")]
[Authorize]
public class ScenesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    [HttpGet]
    public async Task<IActionResult> GetScenes()
    {
        var userId = GetClerkUserId();
        var scenes = await context.Scenes
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SceneSummary
            {
                Id = s.Id,
                Title = s.Title,
                DisplayOrder = s.DisplayOrder,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
        return Ok(scenes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetScene(int id)
    {
        var userId = GetClerkUserId();
        var scene = await context.Scenes
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
        if (scene == null) return NotFound();
        return Ok(ToFull(scene));
    }

    [HttpPost]
    public async Task<IActionResult> SaveScene([FromBody] SaveSceneRequest request)
    {
        var userId = GetClerkUserId();
        var nextOrder = (await context.Scenes
            .Where(s => s.UserId == userId)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0) + 1;

        var scene = new Scene
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId,
            DisplayOrder = nextOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Scenes.Add(scene);
        await context.SaveChangesAsync();
        return Ok(ToFull(scene));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScene(int id, [FromBody] UpdateSceneRequest request)
    {
        var userId = GetClerkUserId();
        var scene = await context.Scenes.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
        if (scene == null) return NotFound();
        scene.Title = request.Title;
        scene.Content = request.Content;
        scene.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Ok(ToFull(scene));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScene(int id)
    {
        var userId = GetClerkUserId();
        var scene = await context.Scenes.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
        if (scene == null) return NotFound();
        context.Scenes.Remove(scene);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderScenes([FromBody] ReorderRequest request)
    {
        var userId = GetClerkUserId();
        var scenes = await context.Scenes.Where(s => s.UserId == userId).ToListAsync();
        for (int i = 0; i < request.OrderedIds.Length; i++)
        {
            var scene = scenes.FirstOrDefault(s => s.Id == request.OrderedIds[i]);
            if (scene is not null) scene.DisplayOrder = i + 1;
        }
        await context.SaveChangesAsync();
        return NoContent();
    }

    private static SceneFull ToFull(Scene s) => new()
    {
        Id = s.Id,
        Title = s.Title,
        Content = s.Content,
        DisplayOrder = s.DisplayOrder,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}

public class SaveSceneRequest { public string Title { get; set; } = null!; public string Content { get; set; } = null!; }
public class UpdateSceneRequest { public string Title { get; set; } = null!; public string Content { get; set; } = null!; }
public class ReorderRequest { public int[] OrderedIds { get; set; } = null!; }
public class SceneSummary { public int Id { get; set; } public string Title { get; set; } = null!; public int DisplayOrder { get; set; } public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } }
public class SceneFull : SceneSummary { public string Content { get; set; } = null!; }
