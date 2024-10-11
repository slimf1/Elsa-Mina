namespace ElsaMina.Core.Models;

public interface IConfiguration
{
    string Env { get; }
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
}