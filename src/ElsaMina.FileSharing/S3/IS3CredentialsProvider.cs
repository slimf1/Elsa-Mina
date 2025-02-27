namespace ElsaMina.FileSharing.S3;

public interface IS3CredentialsProvider
{
    string BucketName { get; }
    string EndpointUrl { get; }
    string AccessKey { get; }
    string SecretKey { get; }
    string BaseUrl { get; }
}