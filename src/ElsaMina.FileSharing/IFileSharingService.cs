namespace ElsaMina.FileSharing;

public interface IFileSharingService : IDisposable
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task<string?> CreateFileAsync(Stream? fileStream = default, string? fileName = default,
        string? description = default, string? mimeType = default, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
}