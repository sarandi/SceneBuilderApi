namespace SceneBuilderApi.Models;

public class Story
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? UniverseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public Universe? Universe { get; set; }
    public ICollection<Scene> Scenes { get; set; } = new List<Scene>();
}
