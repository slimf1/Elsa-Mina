namespace ElsaMina.Core.Services.Files;

public interface IFilesService
{
    bool FileExists(string filePath);
    Task<string> ReadTextAsync(string filePath);
}