namespace SceneBuilderApi.Models;

public class StoryUniverse
{
    public int StoryId { get; set; }
    public int UniverseId { get; set; }
    public Story Story { get; set; } = null!;
    public Universe Universe { get; set; } = null!;
}
