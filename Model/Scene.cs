namespace SceneBuilderApi.Models;

public class Scene
{
    public int Id { get; set; }
    public int StoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Story Story { get; set; } = null!;
    public ICollection<SceneUniverse> SceneUniverses { get; set; } = new List<SceneUniverse>();
}
