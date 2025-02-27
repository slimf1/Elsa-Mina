using ElsaMina.FileSharing.S3;

namespace ElsaMina.Core.Services.Config;

public class S3CredentialsProvider : IS3CredentialsProvider
{
    private readonly IConfigurationManager _configurationManager;

    public S3CredentialsProvider(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    public string BucketName => _configurationManager.Configuration.S3BucketName;

    public string EndpointUrl => _configurationManager.Configuration.S3EndpointUrl;

    public string AccessKey => _configurationManager.Configuration.S3AccessKey;

    public string SecretKey => _configurationManager.Configuration.S3SecretKey;

    public string BaseUrl => _configurationManager.Configuration.S3BaseUrl;
}