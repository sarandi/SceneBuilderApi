namespace SceneBuilderApi.Models;

public class User
{
    public string Id { get; set; } = string.Empty; // Clerk user ID (e.g. "user_2abc...")
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Story> Stories { get; set; } = new List<Story>();
}