namespace ElsaMina.FileSharing;

public interface IFileSharingService : IDisposable
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<string?> CreateFileAsync(byte[]? fileContent = default, string? fileName = default,
        string? description = default, string? mimeType = default, CancellationToken cancellationToken = default);
    
    Task<string?> CreateFileAsync(Stream? fileContent = default, string? fileName = default,
        string? description = default, string? mimeType = default, CancellationToken cancellationToken = default);
}