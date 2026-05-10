namespace SceneBuilderApi.Models;

// Stores text, text_array (as JSON), number, and boolean field values
public class EntityFieldValue
{
    public int Id { get; set; }
    public int EntityId { get; set; }
    public int FieldId { get; set; }
    public string? TextValue { get; set; }   // text and text_array (JSON-encoded string[])
    public double? NumberValue { get; set; }
    public bool? BoolValue { get; set; }
    public bool IsSecret { get; set; }
    public Entity Entity { get; set; } = null!;
    public EntityTypeField Field { get; set; } = null!;
}
