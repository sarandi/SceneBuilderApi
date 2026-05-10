using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/entity-types")]
[Authorize]
public class EntityTypesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    // Returns built-in types + types created by this user, each with their fields
    [HttpGet]
    public async Task<IActionResult> GetEntityTypes()
    {
        var userId = GetClerkUserId();
        var types = await context.EntityTypes
            .Where(t => t.IsBuiltIn || t.UserId == userId)
            .Include(t => t.Fields.OrderBy(f => f.DisplayOrder))
            .OrderBy(t => t.IsBuiltIn ? 0 : 1)
            .ThenBy(t => t.Name)
            .Select(t => new EntityTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                Icon = t.Icon,
                Color = t.Color,
                IsBuiltIn = t.IsBuiltIn,
                Fields = t.Fields.OrderBy(f => f.DisplayOrder).Select(f => new EntityTypeFieldDto
                {
                    Id = f.Id,
                    Key = f.Key,
                    Label = f.Label,
                    ValueType = f.ValueType,
                    RefEntityTypeId = f.RefEntityTypeId,
                    IsBuiltIn = f.IsBuiltIn,
                    IsRequired = f.IsRequired,
                    IsGmOnly = f.IsGmOnly,
                    DisplayOrder = f.DisplayOrder,
                }).ToList()
            })
            .ToListAsync();

        return Ok(types);
    }
}

public class EntityTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsBuiltIn { get; set; }
    public List<EntityTypeFieldDto> Fields { get; set; } = [];
}

public class EntityTypeFieldDto
{
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string ValueType { get; set; } = null!;
    public int? RefEntityTypeId { get; set; }
    public bool IsBuiltIn { get; set; }
    public bool IsRequired { get; set; }
    public bool IsGmOnly { get; set; }
    public int DisplayOrder { get; set; }
}
