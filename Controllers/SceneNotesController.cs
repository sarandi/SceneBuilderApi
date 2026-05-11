using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/stories/{storyId}/scenes/{sceneId}/notes")]
[Authorize]
public class SceneNotesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    private async Task<bool> SceneBelongsToUser(int storyId, int sceneId, string userId) =>
        await context.Scenes.AnyAsync(s => s.Id == sceneId && s.StoryId == storyId && s.Story.UserId == userId);

    [HttpGet]
    public async Task<IActionResult> GetNotes(int storyId, int sceneId)
    {
        var userId = GetClerkUserId();
        if (!await SceneBelongsToUser(storyId, sceneId, userId)) return NotFound();

        var notes = await context.SceneNotes
            .Where(n => n.SceneId == sceneId && n.UserId == userId)
            .OrderBy(n => n.CreatedAt)
            .Select(n => new SceneNoteDto { Id = n.Id, Title = n.Title, Content = n.Content, CreatedAt = n.CreatedAt, UpdatedAt = n.UpdatedAt })
            .ToListAsync();

        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote(int storyId, int sceneId, [FromBody] SceneNoteRequest request)
    {
        var userId = GetClerkUserId();
        if (!await SceneBelongsToUser(storyId, sceneId, userId)) return NotFound();

        var note = new SceneNote
        {
            SceneId = sceneId,
            UserId = userId,
            Title = request.Title ?? string.Empty,
            Content = request.Content ?? string.Empty,
        };
        context.SceneNotes.Add(note);
        await context.SaveChangesAsync();

        return Ok(new SceneNoteDto { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt, UpdatedAt = note.UpdatedAt });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int storyId, int sceneId, int id, [FromBody] SceneNoteRequest request)
    {
        var userId = GetClerkUserId();
        var note = await context.SceneNotes.FirstOrDefaultAsync(n => n.Id == id && n.SceneId == sceneId && n.UserId == userId);
        if (note == null) return NotFound();

        note.Title = request.Title ?? note.Title;
        note.Content = request.Content ?? note.Content;
        note.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Ok(new SceneNoteDto { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt, UpdatedAt = note.UpdatedAt });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int storyId, int sceneId, int id)
    {
        var userId = GetClerkUserId();
        var note = await context.SceneNotes.FirstOrDefaultAsync(n => n.Id == id && n.SceneId == sceneId && n.UserId == userId);
        if (note == null) return NotFound();

        context.SceneNotes.Remove(note);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public class SceneNoteDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SceneNoteRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
}
