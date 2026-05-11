namespace SceneBuilderApi.Models;

public class SceneNote
{
    public int Id { get; set; }
    public int SceneId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Scene Scene { get; set; } = null!;
    public User User { get; set; } = null!;
}
