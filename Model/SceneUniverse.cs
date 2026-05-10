namespace SceneBuilderApi.Models;

public class SceneUniverse
{
    public int SceneId { get; set; }
    public int UniverseId { get; set; }
    public Scene Scene { get; set; } = null!;
    public Universe Universe { get; set; } = null!;
}
