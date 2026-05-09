using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScenesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ScenesController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID not found in token."));

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var scenes = await _db.Scenes
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.DisplayOrder)
            .ThenByDescending(s => s.UpdatedAt)
            .Select(s => new
            {
                s.Id,
                s.Title,
                s.DisplayOrder,
                s.CreatedAt,
                s.UpdatedAt,
            })
            .ToListAsync();

        return Ok(scenes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var scene = await _db.Scenes
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (scene == null) return NotFound();
        return Ok(scene);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SceneRequest request)
    {
        var userId = GetUserId();
        var maxOrder = await _db.Scenes
            .Where(s => s.UserId == userId)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0;

        var scene = new Scene
        {
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            DisplayOrder = maxOrder + 1,
        };

        _db.Scenes.Add(scene);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = scene.Id }, scene);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SceneRequest request)
    {
        var userId = GetUserId();
        var scene = await _db.Scenes
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (scene == null) return NotFound();

        scene.Title = request.Title;
        scene.Content = request.Content;
        scene.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(scene);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var scene = await _db.Scenes
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (scene == null) return NotFound();

        _db.Scenes.Remove(scene);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequest request)
    {
        var userId = GetUserId();
        var scenes = await _db.Scenes
            .Where(s => s.UserId == userId)
            .ToListAsync();

        for (int i = 0; i < request.OrderedIds.Count; i++)
        {
            var scene = scenes.FirstOrDefault(s => s.Id == request.OrderedIds[i]);
            if (scene != null) scene.DisplayOrder = i;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record SceneRequest(string Title, string Content);
public record ReorderRequest(List<int> OrderedIds);