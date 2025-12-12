using ElsaMina.FileSharing.S3;
using ElsaMina.Logging;

namespace ElsaMina.Core.Services.Config;

public interface IConfiguration : IS3CredentialsProvider, ILoggingConfiguration
{
    string Host { get; }
    string Port { get; }
    string Name { get; }
    string Password { get; }
    string Trigger { get; }
    IEnumerable<string> Rooms { get; }
    IEnumerable<string> RoomBlacklist { get; }
    IEnumerable<string> Whitelist { get; }
    string Avatar { get; }
    string DefaultRoom { get; }
    string DefaultLocaleCode { get; }
    string ConnectionString { get; }
    int DatabaseMaxRetries { get; }
    TimeSpan DatabaseRetryDelay { get; }
    string YoutubeApiKey { get; }
    string DictionaryApiKey { get; }
    string RiotApiKey { get; }
    string GeniusApiKey { get; }
    string ArcadeWebhookUrl { get; }
    string MistralApiKey { get; }
    string ChatGptApiKey { get; }
    string GeminiApiKey { get; }
    string ElevenLabsApiKey { get; }
    TimeSpan PlayTimeUpdatesInterval { get; }
    string SheetsAccessAccountCredentialsFile { get; set; }
    IReadOnlyDictionary<string, IEnumerable<string>> TourAnnounces { get; }
}