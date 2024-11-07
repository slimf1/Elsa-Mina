namespace ElsaMina.Core.Services.Images;

public interface IImageService
{
    Task<(int Width, int Height)> GetRemoteImageDimensions(string url);
    (int Width, int Height) ResizeWithSameAspectRatio(int width, int height, int maxWidth, int maxHeight);
}