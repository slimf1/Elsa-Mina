using ElsaMina.FileSharing.S3;

namespace ElsaMina.Core.Services.Config;

public interface IConfiguration : IS3CredentialsProvider
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
}