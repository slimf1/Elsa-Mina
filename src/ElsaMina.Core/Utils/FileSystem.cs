﻿namespace ElsaMina.Core.Utils;

public static class FileSystem
{
    public static IEnumerable<string> GetFilesFromDirectoryRecursively(string path)
    {
        var queue = new Queue<string>();
        queue.Enqueue(path);

        while (queue.Count > 0)
        {
            var currentPath = queue.Dequeue();
            foreach (var subDirectory in Directory.GetDirectories(currentPath)) {
                queue.Enqueue(subDirectory);
            }
            foreach (var file in Directory.GetFiles(currentPath))
            {
                yield return file;
            }
        }
    }
}