namespace SceneBuilderApi.Models;

public class Universe
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public ICollection<StoryUniverse> StoryUniverses { get; set; } = new List<StoryUniverse>();
    public ICollection<SceneUniverse> SceneUniverses { get; set; } = new List<SceneUniverse>();
    public ICollection<Entity> Entities { get; set; } = new List<Entity>();
    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();
}
