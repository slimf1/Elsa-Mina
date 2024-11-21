namespace ElsaMina.Core.Models;

public class Configuration : IConfiguration
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Trigger { get; set; }
    public IEnumerable<string> Rooms { get; set; }
    public IEnumerable<string> RoomBlacklist { get; set; }
    public IEnumerable<string> Whitelist { get; set; }
    public string DefaultRoom { get; set; }
    public string DefaultLocaleCode { get; set; }
    public string ConnectionString { get; set; }
    public string YoutubeApiKey { get; set; }
    public string DictionaryApiKey { get; set; }
    public string RiotApiKey { get; set; }
    public string ArcadeWebhookUrl { get; set; }
}