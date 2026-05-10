namespace SceneBuilderApi.Models;

public class CalendarEra
{
    public int Id { get; set; }
    public int CalendarId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartDay { get; set; }
    public int? EndDay { get; set; } // null = current era
    public string? Description { get; set; }
    public Calendar Calendar { get; set; } = null!;
}
