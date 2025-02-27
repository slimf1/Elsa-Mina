namespace ElsaMina.Core.Models;

public interface IConfiguration
{
    string Host { get; }
    string Port { get; }
    string Name { get; }
    string Password { get; }
    string Trigger { get; }
    IEnumerable<string> Rooms { get; }
    IEnumerable<string> RoomBlacklist { get; }
    IEnumerable<string> Whitelist { get; }
    string DefaultRoom { get; }
    string DefaultLocaleCode { get; }
    string ConnectionString { get; }
    string YoutubeApiKey { get; }
    string DictionaryApiKey { get; }
    string RiotApiKey { get; }
    string ArcadeWebhookUrl { get; }
    string MistralApiKey { get; }
    string ElevenLabsApiKey { get; }
    string S3BucketName { get; }
    string S3EndpointUrl { get; }
    string S3AccessKey { get; }
    string S3SecretKey { get; }
    string S3BaseUrl { get; }
}