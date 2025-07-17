using System.Net;
using System.Security.Cryptography;
using Amazon.S3;
using Amazon.S3.Model;

namespace ElsaMina.FileSharing.S3;

public class S3FileSharingService : IFileSharingService
{
    private readonly IS3CredentialsProvider _credentialsProvider;
    private AmazonS3Client? _client;
    private bool _disposed;

    public S3FileSharingService(IS3CredentialsProvider credentialsProvider)
    {
        _credentialsProvider = credentialsProvider;
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = _credentialsProvider.S3EndpointUrl,
            ForcePathStyle = true
        };

        _client = new AmazonS3Client(_credentialsProvider.S3AccessKey, _credentialsProvider.S3SecretKey, config);
        return Task.CompletedTask;
    }

    public Task<string?> CreateFileAsync(byte[]? fileContent = default, string? fileName = default,
        string? description = default,
        string? mimeType = default, CancellationToken cancellationToken = default)
    {
        return CreateFileAsync(fileContent != null ? new MemoryStream(fileContent) : null,
            fileName, description, mimeType, cancellationToken);
    }

    public async Task<string?> CreateFileAsync(Stream? fileContent = default, string? fileName = default,
        string? description = default,
        string? mimeType = default, CancellationToken cancellationToken = default)
    {
        if (_client == null || fileName == null || fileContent == null)
        {
            return null;
        }

        var sha256 = ComputeSha256(fileContent);
        var request = new PutObjectRequest
        {
            BucketName = _credentialsProvider.S3BucketName,
            Key = fileName,
            InputStream = fileContent,
            ContentType = mimeType,
            ChecksumSHA256 = sha256,
            CannedACL = S3CannedACL.PublicRead
        };
        request.Metadata.Add("description", description);

        var response = await _client.PutObjectAsync(request, cancellationToken);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"Upload failed: {response.HttpStatusCode}");
            return null;
        }

        return GetUrlFromObject(fileName);
    }

    private string GetUrlFromObject(string objectName)
    {
        var baseUrl = new Uri(_credentialsProvider.S3BaseUrl);
        return new Uri(baseUrl, objectName).AbsoluteUri;
    }

    private static string ComputeSha256(byte[] bytes)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
    
    private static string ComputeSha256(Stream bytes)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(bytes);
        bytes.Seek(0, SeekOrigin.Begin);
        return Convert.ToBase64String(hashBytes);
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

        _client?.Dispose();
        _disposed = true;
    }
}