using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace ElsaMina.FileSharing.GoogleDrive;

public class GoogleDriveFileSharingService : IFileSharingService
{
    private static readonly string[] SCOPES = [DriveService.Scope.Drive];
    private const string CREDENTIALS_FILE = "credentials.json"; // TODO : laisser le client en décider (cf sheets)

    private DriveService? _driveService;
    private bool _disposed;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        GoogleCredential? credential;
        await using (var stream = new FileStream(CREDENTIALS_FILE, FileMode.Open, FileAccess.Read))
        {
            var secrets = await GoogleCredential.FromStreamAsync(stream, cancellationToken);
            credential = secrets.CreateScoped(SCOPES);
        }

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "ElsaMina"
        });
    }

    public async Task<string?> CreateFileAsync(Stream? fileContent = default,
        string? fileName = default,
        string? description = default,
        string? mimeType = default,
        CancellationToken cancellationToken = default)
    {
        if (_driveService == null || fileContent == null || fileName == null)
        {
            return null;
        }

        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = fileName,
            Description = description
        };

        var request = _driveService.Files.Create(fileMetadata, fileContent, mimeType);
        request.Fields = "id";
        await request.UploadAsync(cancellationToken);

        var file = request.ResponseBody;
        ShareFile(file);
        return GetFileUrlFromId(file.Id);
    }
    
    public Task<string?> CreateFileAsync(byte[]? fileContent = default,
            string? fileName = default,
            string? description = default,
            string? mimeType = default,
            CancellationToken cancellationToken = default)
        {
            return CreateFileAsync(
                fileContent != null ? new MemoryStream(fileContent) : null,
                fileName, description, mimeType, cancellationToken);
        }

    private void ShareFile(Google.Apis.Drive.v3.Data.File file)
    {
        if (_driveService == null)
        {
            return;
        }

        var permission = new Google.Apis.Drive.v3.Data.Permission
        {
            Type = "anyone",
            Role = "reader"
        };

        _driveService.Permissions.Create(permission, file.Id).Execute();
        Console.WriteLine($"File shared: {GetFileUrlFromId(file.Id)}");
    }

    private static string GetFileUrlFromId(string id)
    {
        return $"https://drive.google.com/file/d/{id}/view";
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }
        
        _driveService?.Dispose();
        _disposed = true;
    }
}