using System.Text.RegularExpressions;
using SixLabors.ImageSharp;

namespace ElsaMina.Core.Utils;

public static class Images
{
    public static readonly Regex IMAGE_LINK_REGEX = new("(http)?s?:(//[^\"']*.(?:png|jpg|jpeg|gif|png|svg))");
    private static readonly HttpClient HTTP_CLIENT = new();
    
    public static async Task<(int Width, int Height)> GetRemoteImageDimensions(string url)
    {
        try
        {
            var stream = await HTTP_CLIENT.GetStreamAsync(url);
            var image = await Image.LoadAsync(stream);
            return (image.Width, image.Height);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to load image");
            return (-1, -1);
        }
    }

    public static (int Width, int Height) ResizeWithSameAspectRatio(int width, int height, int maxWidth, int maxHeight)
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