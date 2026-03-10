namespace ElsaMina.Commands.Profile;

public interface IProfileService
{
    Task<string> GetProfileHtmlAsync(string userId, string roomId, CancellationToken cancellationToken = default);
}
