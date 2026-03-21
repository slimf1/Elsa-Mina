namespace ElsaMina.Core.Services.Images;

public interface IImageService
{
    Task<(int Width, int Height)> GetRemoteImageDimensions(string url, CancellationToken cancellationToken = default);
}