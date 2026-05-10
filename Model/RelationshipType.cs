namespace SceneBuilderApi.Models;

public class RelationshipType
{
    public int Id { get; set; }
    public string? UserId { get; set; } // null = built-in default
    public string Name { get; set; } = string.Empty;
    public bool IsBuiltIn { get; set; }
    public User? User { get; set; }
    public ICollection<EntityRelationship> Relationships { get; set; } = new List<EntityRelationship>();
}
