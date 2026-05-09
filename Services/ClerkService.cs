using System.Text;
using System.Text.Json;

namespace SceneBuilderApi.Services;

public class ClerkService(HttpClient http) : IClerkService
{
    public async Task UpdateUserRoleAsync(string clerkUserId, string role)
    {
        var payload = JsonSerializer.Serialize(new { public_metadata = new { role } });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await http.PatchAsync($"users/{clerkUserId}", content);
        response.EnsureSuccessStatusCode();
    }
}
