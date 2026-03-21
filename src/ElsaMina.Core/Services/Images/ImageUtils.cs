namespace ElsaMina.Core.Services.Images;

public static class ImageUtils
{
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