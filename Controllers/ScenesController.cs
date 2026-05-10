using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/stories/{storyId}/scenes")]
[Authorize]
public class ScenesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    private async Task<bool> StoryBelongsToUser(int storyId, string userId) =>
        await context.Stories.AnyAsync(s => s.Id == storyId && s.UserId == userId);

    [HttpGet]
    public async Task<IActionResult> GetScenes(int storyId)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var scenes = await context.Scenes
            .Where(s => s.StoryId == storyId)
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SceneSummary
            {
                Id = s.Id, Title = s.Title, DisplayOrder = s.DisplayOrder,
                UniverseIds = s.SceneUniverses.Select(su => su.UniverseId).ToList(),
                CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
        return Ok(scenes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetScene(int storyId, int id)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var scene = await context.Scenes
            .Include(s => s.SceneUniverses)
            .FirstOrDefaultAsync(s => s.Id == id && s.StoryId == storyId);
        if (scene == null) return NotFound();
        return Ok(ToFull(scene));
    }

    [HttpPost]
    public async Task<IActionResult> SaveScene(int storyId, [FromBody] SaveSceneRequest request)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var nextOrder = (await context.Scenes
            .Where(s => s.StoryId == storyId)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0) + 1;

        var universeIds = request.UniverseIds;
        if (universeIds.Count == 0)
        {
            universeIds = await context.StoryUniverses
                .Where(su => su.StoryId == storyId)
                .Select(su => su.UniverseId)
                .ToListAsync();
        }

        var scene = new Scene
        {
            StoryId = storyId,
            Title = request.Title,
            Content = request.Content,
            DisplayOrder = nextOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Scenes.Add(scene);
        await context.SaveChangesAsync();

        foreach (var uid in universeIds)
            context.SceneUniverses.Add(new SceneUniverse { SceneId = scene.Id, UniverseId = uid });

        await context.Stories
            .Where(s => s.Id == storyId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.UpdatedAt, DateTime.UtcNow));

        await context.SaveChangesAsync();
        await context.Entry(scene).Collection(s => s.SceneUniverses).LoadAsync();
        return Ok(ToFull(scene));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateScene(int storyId, int id, [FromBody] UpdateSceneRequest request)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var scene = await context.Scenes
            .Include(s => s.SceneUniverses)
            .FirstOrDefaultAsync(s => s.Id == id && s.StoryId == storyId);
        if (scene == null) return NotFound();

        scene.Title = request.Title;
        scene.Content = request.Content;
        scene.UpdatedAt = DateTime.UtcNow;

        context.SceneUniverses.RemoveRange(scene.SceneUniverses);
        foreach (var uid in request.UniverseIds)
            context.SceneUniverses.Add(new SceneUniverse { SceneId = id, UniverseId = uid });

        await context.Stories
            .Where(s => s.Id == storyId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.UpdatedAt, DateTime.UtcNow));

        await context.SaveChangesAsync();
        await context.Entry(scene).Collection(s => s.SceneUniverses).LoadAsync();
        return Ok(ToFull(scene));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScene(int storyId, int id)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var scene = await context.Scenes.FirstOrDefaultAsync(s => s.Id == id && s.StoryId == storyId);
        if (scene == null) return NotFound();
        context.Scenes.Remove(scene);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderScenes(int storyId, [FromBody] ReorderRequest request)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var scenes = await context.Scenes.Where(s => s.StoryId == storyId).ToListAsync();
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
        UniverseIds = s.SceneUniverses.Select(su => su.UniverseId).ToList(),
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}

public class SaveSceneRequest { public string Title { get; set; } = null!; public string Content { get; set; } = null!; public List<int> UniverseIds { get; set; } = new(); }
public class UpdateSceneRequest { public string Title { get; set; } = null!; public string Content { get; set; } = null!; public List<int> UniverseIds { get; set; } = new(); }
public class ReorderRequest { public int[] OrderedIds { get; set; } = null!; }
public class SceneSummary { public int Id { get; set; } public string Title { get; set; } = null!; public int DisplayOrder { get; set; } public List<int> UniverseIds { get; set; } = new(); public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } }
public class SceneFull : SceneSummary { public string Content { get; set; } = null!; }
