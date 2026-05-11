namespace SceneBuilderApi.Models;

public class StoryNote
{
    public int Id { get; set; }
    public int StoryId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Story Story { get; set; } = null!;
    public User User { get; set; } = null!;
}
