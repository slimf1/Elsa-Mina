namespace ElsaMina.Core.Utils;

public static class LongExtensions
{
    public static string ToReadableDataSize(this long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB", "PB"];
        var len = bytes;
        var order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}