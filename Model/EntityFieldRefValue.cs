namespace SceneBuilderApi.Models;

// Stores entity_ref and entity_ref_list field values
public class EntityFieldRefValue
{
    public int Id { get; set; }
    public int EntityId { get; set; }
    public int FieldId { get; set; }
    public int RefEntityId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsSecret { get; set; }
    public Entity Entity { get; set; } = null!;
    public EntityTypeField Field { get; set; } = null!;
    public Entity RefEntity { get; set; } = null!;
}
