using System.Security.Cryptography;
using Amazon.S3;
using Amazon.S3.Model;

namespace ElsaMina.FileSharing.S3;

public class S3FileSharingService : IFileSharingService
{
    private readonly IS3CredentialsProvider _credentialsProvider;
    private AmazonS3Client? _client;

    public S3FileSharingService(IS3CredentialsProvider credentialsProvider)
    {
        _credentialsProvider = credentialsProvider;
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = _credentialsProvider.EndpointUrl,
            ForcePathStyle = true
        };

        _client = new AmazonS3Client(_credentialsProvider.AccessKey, _credentialsProvider.SecretKey, config);
        return Task.CompletedTask;
    }

    public async Task<string?> CreateFileAsync(Stream? fileStream = default, string? fileName = default, string? description = default,
        string? mimeType = default, CancellationToken cancellationToken = default)
    {
        if (_client == null || fileName == null || fileStream == null)
        {
            return null;
        }

        var sha256 = ComputeSha256(fileStream);
        fileStream.Position = 0; // Reset stream for upload
        var request = new PutObjectRequest
        {
            BucketName = _credentialsProvider.BucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = mimeType,
            ChecksumSHA256 = sha256,
            CannedACL = S3CannedACL.PublicRead // Makes the object publicly accessible
        };
        request.Metadata.Add("description", description);

        var response = await _client.PutObjectAsync(request, cancellationToken);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine($"Upload failed: {response.HttpStatusCode}");
            return null;
        }

        return GetUrlFromObject(fileName);
    }

    private string GetUrlFromObject(string objectName)
    {
        var baseUrl = new Uri(_credentialsProvider.BaseUrl);
        return new Uri(baseUrl, objectName).AbsoluteUri;
    }

    public Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
    
    private static string ComputeSha256(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convert to hex string
    }

}