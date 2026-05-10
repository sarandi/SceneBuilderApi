namespace SceneBuilderApi.Models;

public class Entity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? UniverseId { get; set; }
    public int EntityTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    public bool IsSecret { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public Universe? Universe { get; set; }
    public EntityType EntityType { get; set; } = null!;
    public ICollection<EntityFieldValue> FieldValues { get; set; } = new List<EntityFieldValue>();
    public ICollection<EntityFieldRefValue> FieldRefValues { get; set; } = new List<EntityFieldRefValue>();
    public ICollection<EntityRelationship> OutgoingRelationships { get; set; } = new List<EntityRelationship>();
    public ICollection<EntityRelationship> IncomingRelationships { get; set; } = new List<EntityRelationship>();
}
