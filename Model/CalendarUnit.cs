namespace SceneBuilderApi.Models;

// Direction: above_base | base | below_base
// Type: fixed_division | named_period | event_anchored
public class CalendarUnit
{
    public int Id { get; set; }
    public int CalendarId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PluralName { get; set; }
    public int? ParentUnitId { get; set; }
    public string Direction { get; set; } = "above_base";
    public string Type { get; set; } = "fixed_division";
    public int? CountPerParent { get; set; } // null if irregular or event-anchored
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public Calendar Calendar { get; set; } = null!;
    public CalendarUnit? ParentUnit { get; set; }
    public ICollection<CalendarUnit> ChildUnits { get; set; } = new List<CalendarUnit>();
}
