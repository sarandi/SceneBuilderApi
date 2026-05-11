using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/stories/{storyId}/notes")]
[Authorize]
public class StoryNotesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    private async Task<bool> StoryBelongsToUser(int storyId, string userId) =>
        await context.Stories.AnyAsync(s => s.Id == storyId && s.UserId == userId);

    [HttpGet]
    public async Task<IActionResult> GetNotes(int storyId)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var notes = await context.StoryNotes
            .Where(n => n.StoryId == storyId && n.UserId == userId)
            .OrderBy(n => n.CreatedAt)
            .Select(n => new StoryNoteDto { Id = n.Id, Title = n.Title, Content = n.Content, CreatedAt = n.CreatedAt, UpdatedAt = n.UpdatedAt })
            .ToListAsync();

        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote(int storyId, [FromBody] StoryNoteRequest request)
    {
        var userId = GetClerkUserId();
        if (!await StoryBelongsToUser(storyId, userId)) return NotFound();

        var note = new StoryNote
        {
            StoryId = storyId,
            UserId = userId,
            Title = request.Title ?? string.Empty,
            Content = request.Content ?? string.Empty,
        };
        context.StoryNotes.Add(note);
        await context.SaveChangesAsync();

        return Ok(new StoryNoteDto { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt, UpdatedAt = note.UpdatedAt });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int storyId, int id, [FromBody] StoryNoteRequest request)
    {
        var userId = GetClerkUserId();
        var note = await context.StoryNotes.FirstOrDefaultAsync(n => n.Id == id && n.StoryId == storyId && n.UserId == userId);
        if (note == null) return NotFound();

        note.Title = request.Title ?? note.Title;
        note.Content = request.Content ?? note.Content;
        note.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Ok(new StoryNoteDto { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt, UpdatedAt = note.UpdatedAt });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int storyId, int id)
    {
        var userId = GetClerkUserId();
        var note = await context.StoryNotes.FirstOrDefaultAsync(n => n.Id == id && n.StoryId == storyId && n.UserId == userId);
        if (note == null) return NotFound();

        context.StoryNotes.Remove(note);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public class StoryNoteDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class StoryNoteRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
}
