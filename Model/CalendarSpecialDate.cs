namespace SceneBuilderApi.Models;

public class CalendarSpecialDate
{
    public int Id { get; set; }
    public int CalendarId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public int? DayOfYear { get; set; }   // if recurring: which day within the year cycle
    public int? AbsoluteDay { get; set; } // if one-time: universal day integer
    public string? Description { get; set; }
    public string? Significance { get; set; }
    public Calendar Calendar { get; set; } = null!;
}
