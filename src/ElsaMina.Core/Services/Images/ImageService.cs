using System.Text.RegularExpressions;
using ElsaMina.Core.Services.Http;
using SixLabors.ImageSharp;

namespace ElsaMina.Core.Services.Images;

public class ImageService : IImageService
{
    private static readonly Regex IMAGE_LINK_REGEX = new("(http)?s?:(//[^\"']*.(?:png|jpg|jpeg|gif|png|svg))",
        RegexOptions.Compiled, Constants.REGEX_MATCH_TIMEOUT);

    private readonly IHttpService _httpService;

    public ImageService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public bool IsImageLink(string link) =>
        !string.IsNullOrWhiteSpace(link) && IMAGE_LINK_REGEX.IsMatch(link);

    public async Task<(int Width, int Height)> GetRemoteImageDimensions(string url)
    {
        try
        {
            var stream = await _httpService.GetStreamAsync(url);
            var image = await Image.LoadAsync(stream);
            return (image.Width, image.Height);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to load image");
            return (-1, -1);
        }
    }

    public (int Width, int Height) ResizeWithSameAspectRatio(int width, int height, int maxWidth, int maxHeight)
    {
        int newWidth, newHeight;
        if (width <= maxWidth && height <= maxHeight)
        {
            newWidth = width;
            newHeight = height;
        }
        else if ((double)width / height > (double)maxWidth / maxHeight)
        {
            var ratio = (double)maxWidth / width;
            newWidth = maxWidth;
            newHeight = (int)Math.Round(height * ratio);
        }
        else
        {
            var ratio = (double)maxHeight / height;
            newHeight = maxHeight;
            newWidth = (int)Math.Round(width * ratio);
        }

        return (newWidth, newHeight);
    }
}