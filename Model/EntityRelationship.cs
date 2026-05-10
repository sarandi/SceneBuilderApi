namespace SceneBuilderApi.Models;

public class EntityRelationship
{
    public int Id { get; set; }
    public int FromEntityId { get; set; }
    public int ToEntityId { get; set; }
    public int RelationshipTypeId { get; set; }
    public string? Description { get; set; }
    public bool IsSecret { get; set; }
    public Entity FromEntity { get; set; } = null!;
    public Entity ToEntity { get; set; } = null!;
    public RelationshipType RelationshipType { get; set; } = null!;
}
