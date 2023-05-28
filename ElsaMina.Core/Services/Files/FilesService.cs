namespace ElsaMina.Core.Services.Files;

public class FilesService : IFilesService
{
    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public Task<string> ReadTextAsync(string filePath)
    {
        return File.ReadAllTextAsync(filePath);
    }
}