using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace ElsaMina.FileSharing.GoogleDrive;

public class GoogleDriveFileSharingService : IFileSharingService
{
    private static readonly string[] SCOPES = [DriveService.Scope.Drive];
    private const string CREDENTIALS_FILE = "credentials.json";

    private DriveService? _driveService;

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

    public async Task<string?> CreateFileAsync(Stream? fileStream = default,
        string? fileName = default,
        string? description = default,
        string? mimeType = default,
        CancellationToken cancellationToken = default)
    {
        if (_driveService == null)
        {
            throw new ApplicationException("The GoogleDriveFileSharingService is not initialized.");
        }

        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = fileName,
            Description = description
        };

        var request = _driveService.Files.Create(fileMetadata, fileStream, mimeType);
        request.Fields = "id";
        await request.UploadAsync(cancellationToken);

        var file = request.ResponseBody;
        ShareFile(file);
        return GetFileUrlFromId(file.Id);
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

    private string GetFileUrlFromId(string id)
    {
        return $"https://drive.google.com/file/d/{id}/view";
    }

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _driveService?.Dispose();
    }
}