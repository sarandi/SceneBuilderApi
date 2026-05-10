using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Data;
using SceneBuilderApi.Models;
using System.Security.Claims;

namespace SceneBuilderApi.Controllers;

[ApiController]
[Route("api/entities")]
[Authorize]
public class EntitiesController(AppDbContext context) : ControllerBase
{
    private string GetClerkUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("User ID not found in token");

    // List — optionally filter by universeId or entityTypeId
    [HttpGet]
    public async Task<IActionResult> GetEntities([FromQuery] int? universeId, [FromQuery] int? entityTypeId)
    {
        var userId = GetClerkUserId();
        var query = context.Entities
            .Where(e => e.UserId == userId);

        if (universeId.HasValue)
            query = query.Where(e => e.UniverseId == universeId);

        if (entityTypeId.HasValue)
            query = query.Where(e => e.EntityTypeId == entityTypeId);

        var entities = await query
            .OrderBy(e => e.EntityType.Name)
            .ThenBy(e => e.Name)
            .Select(e => new EntitySummaryDto
            {
                Id = e.Id,
                Name = e.Name,
                EntityTypeId = e.EntityTypeId,
                EntityTypeName = e.EntityType.Name,
                EntityTypeIcon = e.EntityType.Icon,
                EntityTypeColor = e.EntityType.Color,
                UniverseId = e.UniverseId,
                IsPublic = e.IsPublic,
                IsSecret = e.IsSecret,
                UpdatedAt = e.UpdatedAt,
            })
            .ToListAsync();

        return Ok(entities);
    }

    // Single entity with all field values
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntity(int id)
    {
        var userId = GetClerkUserId();
        var entity = await context.Entities
            .Where(e => e.Id == id && e.UserId == userId)
            .Include(e => e.EntityType).ThenInclude(t => t.Fields.OrderBy(f => f.DisplayOrder))
            .Include(e => e.FieldValues).ThenInclude(v => v.Field)
            .Include(e => e.FieldRefValues).ThenInclude(v => v.Field)
            .Include(e => e.FieldRefValues).ThenInclude(v => v.RefEntity)
            .FirstOrDefaultAsync();

        if (entity == null) return NotFound();

        return Ok(new EntityDetailDto
        {
            Id = entity.Id,
            Name = entity.Name,
            EntityTypeId = entity.EntityTypeId,
            EntityTypeName = entity.EntityType.Name,
            EntityTypeIcon = entity.EntityType.Icon,
            EntityTypeColor = entity.EntityType.Color,
            UniverseId = entity.UniverseId,
            IsPublic = entity.IsPublic,
            IsSecret = entity.IsSecret,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            FieldValues = entity.FieldValues.Select(v => new FieldValueDto
            {
                FieldId = v.FieldId,
                Key = v.Field.Key,
                ValueType = v.Field.ValueType,
                TextValue = v.TextValue,
                NumberValue = v.NumberValue,
                BoolValue = v.BoolValue,
                IsSecret = v.IsSecret,
            }).ToList(),
            FieldRefValues = entity.FieldRefValues.Select(v => new FieldRefValueDto
            {
                FieldId = v.FieldId,
                Key = v.Field.Key,
                RefEntityId = v.RefEntityId,
                RefEntityName = v.RefEntity.Name,
                RefEntityTypeId = v.RefEntity.EntityTypeId,
                DisplayOrder = v.DisplayOrder,
                IsSecret = v.IsSecret,
            }).ToList(),
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateEntity([FromBody] EntityRequest request)
    {
        var userId = GetClerkUserId();
        var entity = new Entity
        {
            UserId = userId,
            UniverseId = request.UniverseId,
            EntityTypeId = request.EntityTypeId,
            Name = request.Name,
            IsPublic = request.IsPublic,
            IsSecret = request.IsSecret,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        context.Entities.Add(entity);
        await context.SaveChangesAsync();

        await SaveFieldValues(entity.Id, request);

        return Ok(new EntitySummaryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            EntityTypeId = entity.EntityTypeId,
            UniverseId = entity.UniverseId,
            IsPublic = entity.IsPublic,
            IsSecret = entity.IsSecret,
            UpdatedAt = entity.UpdatedAt,
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEntity(int id, [FromBody] EntityRequest request)
    {
        var userId = GetClerkUserId();
        var entity = await context.Entities.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (entity == null) return NotFound();

        entity.Name = request.Name;
        entity.UniverseId = request.UniverseId;
        entity.IsPublic = request.IsPublic;
        entity.IsSecret = request.IsSecret;
        entity.UpdatedAt = DateTime.UtcNow;

        // Replace field values
        var oldValues = context.EntityFieldValues.Where(v => v.EntityId == id);
        var oldRefValues = context.EntityFieldRefValues.Where(v => v.EntityId == id);
        context.EntityFieldValues.RemoveRange(oldValues);
        context.EntityFieldRefValues.RemoveRange(oldRefValues);
        await context.SaveChangesAsync();

        await SaveFieldValues(id, request);

        return Ok(new EntitySummaryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            EntityTypeId = entity.EntityTypeId,
            UniverseId = entity.UniverseId,
            IsPublic = entity.IsPublic,
            IsSecret = entity.IsSecret,
            UpdatedAt = entity.UpdatedAt,
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntity(int id)
    {
        var userId = GetClerkUserId();
        var entity = await context.Entities.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (entity == null) return NotFound();
        context.Entities.Remove(entity);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private async Task SaveFieldValues(int entityId, EntityRequest request)
    {
        if (request.FieldValues?.Count > 0)
        {
            var values = request.FieldValues.Select(v => new EntityFieldValue
            {
                EntityId = entityId,
                FieldId = v.FieldId,
                TextValue = v.TextValue,
                NumberValue = v.NumberValue,
                BoolValue = v.BoolValue,
                IsSecret = v.IsSecret,
            });
            context.EntityFieldValues.AddRange(values);
        }

        if (request.FieldRefValues?.Count > 0)
        {
            var refValues = request.FieldRefValues.Select(v => new EntityFieldRefValue
            {
                EntityId = entityId,
                FieldId = v.FieldId,
                RefEntityId = v.RefEntityId,
                DisplayOrder = v.DisplayOrder,
                IsSecret = v.IsSecret,
            });
            context.EntityFieldRefValues.AddRange(refValues);
        }

        await context.SaveChangesAsync();
    }
}

public class EntitySummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int EntityTypeId { get; set; }
    public string? EntityTypeName { get; set; }
    public string? EntityTypeIcon { get; set; }
    public string? EntityTypeColor { get; set; }
    public int? UniverseId { get; set; }
    public bool IsPublic { get; set; }
    public bool IsSecret { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class EntityDetailDto : EntitySummaryDto
{
    public DateTime CreatedAt { get; set; }
    public List<FieldValueDto> FieldValues { get; set; } = [];
    public List<FieldRefValueDto> FieldRefValues { get; set; } = [];
}

public class FieldValueDto
{
    public int FieldId { get; set; }
    public string Key { get; set; } = null!;
    public string ValueType { get; set; } = null!;
    public string? TextValue { get; set; }
    public double? NumberValue { get; set; }
    public bool? BoolValue { get; set; }
    public bool IsSecret { get; set; }
}

public class FieldRefValueDto
{
    public int FieldId { get; set; }
    public string Key { get; set; } = null!;
    public int RefEntityId { get; set; }
    public string RefEntityName { get; set; } = null!;
    public int RefEntityTypeId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsSecret { get; set; }
}

public class EntityRequest
{
    public string Name { get; set; } = null!;
    public int EntityTypeId { get; set; }
    public int? UniverseId { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsSecret { get; set; } = false;
    public List<FieldValueRequest> FieldValues { get; set; } = [];
    public List<FieldRefValueRequest> FieldRefValues { get; set; } = [];
}

public class FieldValueRequest
{
    public int FieldId { get; set; }
    public string? TextValue { get; set; }
    public double? NumberValue { get; set; }
    public bool? BoolValue { get; set; }
    public bool IsSecret { get; set; }
}

public class FieldRefValueRequest
{
    public int FieldId { get; set; }
    public int RefEntityId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsSecret { get; set; }
}
