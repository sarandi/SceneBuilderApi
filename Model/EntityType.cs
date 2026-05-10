namespace SceneBuilderApi.Models;

public class EntityType
{
    public int Id { get; set; }
    public string? UserId { get; set; } // null = built-in
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsBuiltIn { get; set; }
    public User? User { get; set; }
    public ICollection<EntityTypeField> Fields { get; set; } = new List<EntityTypeField>();
    public ICollection<Entity> Entities { get; set; } = new List<Entity>();
}
