namespace ElsaMina.Commands.Misc.RandomImages;

public interface IUnsplashService
{
    Task<string> GetRandomPhotoUrlAsync(string query, CancellationToken cancellationToken = default);
}
