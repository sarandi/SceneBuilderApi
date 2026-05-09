namespace SceneBuilderApi.Services;

public interface IClerkService
{
    Task UpdateUserRoleAsync(string clerkUserId, string role);
}
