namespace SceneBuilderApi.Models;

public class User
{
    public string Id { get; set; } = string.Empty; // Clerk user ID (e.g. "user_2abc...")
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Scene> Scenes { get; set; } = new List<Scene>();
}