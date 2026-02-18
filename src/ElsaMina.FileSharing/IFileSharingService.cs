namespace ElsaMina.FileSharing;

public interface IFileSharingService : IDisposable
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<string?> CreateFileAsync(byte[]? fileContent = null, string? fileName = null,
        string? description = null, string? mimeType = null, CancellationToken cancellationToken = default);
    
    Task<string?> CreateFileAsync(Stream? fileContent = null, string? fileName = null,
        string? description = null, string? mimeType = null, CancellationToken cancellationToken = default);
}