using ElsaMina.Core.Services.Http;
using ElsaMina.Logging;
using SixLabors.ImageSharp;

namespace ElsaMina.Core.Services.Images;

public class ImageService : IImageService
{
    private readonly IHttpService _httpService;

    public ImageService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<(int Width, int Height)> GetRemoteImageDimensions(string url,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stream = await _httpService.GetStreamAsync(url, cancellationToken);
            var image = await Image.LoadAsync(stream, cancellationToken);
            return (image.Width, image.Height);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to load image");
            return (-1, -1);
        }
    }
}