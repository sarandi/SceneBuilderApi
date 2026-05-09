using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/stories")]
[Authorize]
public class StoriesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    [HttpGet]
    public async Task<IActionResult> GetStories()
    {
        var userId = GetClerkUserId();
        var stories = await context.Stories
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => new StoryDto
            {
                Id = s.Id,
                Title = s.Title,
                SceneCount = s.Scenes.Count(),
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
        return Ok(stories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStory(int id)
    {
        var userId = GetClerkUserId();
        var story = await context.Stories
            .Where(s => s.Id == id && s.UserId == userId)
            .Select(s => new StoryDto { Id = s.Id, Title = s.Title, SceneCount = s.Scenes.Count(), CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt })
            .FirstOrDefaultAsync();
        if (story == null) return NotFound();
        return Ok(story);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStory([FromBody] StoryRequest request)
    {
        var userId = GetClerkUserId();
        var story = new Story
        {
            UserId = userId,
            Title = request.Title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Stories.Add(story);
        await context.SaveChangesAsync();
        return Ok(new StoryDto { Id = story.Id, Title = story.Title, SceneCount = 0, CreatedAt = story.CreatedAt, UpdatedAt = story.UpdatedAt });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStory(int id, [FromBody] StoryRequest request)
    {
        var userId = GetClerkUserId();
        var story = await context.Stories.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
        if (story == null) return NotFound();
        story.Title = request.Title;
        story.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Ok(new StoryDto { Id = story.Id, Title = story.Title, SceneCount = story.Scenes.Count, CreatedAt = story.CreatedAt, UpdatedAt = story.UpdatedAt });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStory(int id)
    {
        var userId = GetClerkUserId();
        var story = await context.Stories.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
        if (story == null) return NotFound();
        context.Stories.Remove(story);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public class StoryRequest { public string Title { get; set; } = null!; }
public class StoryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int SceneCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
