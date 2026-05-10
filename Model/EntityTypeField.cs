namespace SceneBuilderApi.Models;

// ValueType values: text | text_array | number | boolean | entity_ref | entity_ref_list
public class EntityTypeField
{
    public int Id { get; set; }
    public int EntityTypeId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string ValueType { get; set; } = "text";
    public int? RefEntityTypeId { get; set; } // for entity_ref / entity_ref_list fields
    public bool IsBuiltIn { get; set; }
    public bool IsRequired { get; set; }
    public bool IsGmOnly { get; set; }
    public int DisplayOrder { get; set; }
    public EntityType EntityType { get; set; } = null!;
    public EntityType? RefEntityType { get; set; }
}
