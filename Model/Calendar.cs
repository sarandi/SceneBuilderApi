namespace SceneBuilderApi.Models;

public class Calendar
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? UniverseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BaseUnitName { get; set; } = "day";
    public string? BaseUnitDescription { get; set; }
    public int BaseUnitInIntegers { get; set; } = 1;
    public int CurrentDay { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public Universe? Universe { get; set; }
    public ICollection<CalendarUnit> Units { get; set; } = new List<CalendarUnit>();
    public ICollection<CalendarEra> Eras { get; set; } = new List<CalendarEra>();
    public ICollection<CalendarSpecialDate> SpecialDates { get; set; } = new List<CalendarSpecialDate>();
}
