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

    public async Task<string?> CreateFileAsync(Stream? fileStream = default, string? fileName = default,
        string? description = default,
        string? mimeType = default, CancellationToken cancellationToken = default)
    {
        if (_client == null || fileName == null || fileStream == null)
        {
            return null;
        }

        var sha256 = ComputeSha256(fileStream);
        var request = new PutObjectRequest
        {
            BucketName = _credentialsProvider.S3BucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = mimeType,
            ChecksumSHA256 = sha256,
            CannedACL = S3CannedACL.PublicRead // Makes the object publicly accessible
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

    private static string ComputeSha256(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(stream);
        stream.Position = 0;
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convert to hex string
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }

        _client?.Dispose();
        _disposed = true;
    }
}