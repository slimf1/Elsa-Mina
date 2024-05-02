namespace ElsaMina.Core.Utils;

public static class FileSystem
{
    public static IEnumerable<string> GetFilesFromDirectoryRecursively(string path)
    {
        var queue = new Queue<string>();
        queue.Enqueue(path);

        while (queue.Count > 0)
        {
            var currentPath = queue.Dequeue();
            foreach (var subDirectory in Directory.GetDirectories(currentPath))
            {
                queue.Enqueue(subDirectory);
            }

            foreach (var file in Directory.GetFiles(currentPath))
            {
                yield return file;
            }
        }
    }

    public static string MakeRelativePath(string filePath, string referencePath)
    {
        var fileUri = new Uri(filePath);
        var referenceUri = new Uri(referencePath);
        return Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString())
            .Replace(Path.PathSeparator, Path.DirectorySeparatorChar);
    }

    public static string RemoveExtension(this string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return fileName[..^extension.Length];
    }
}