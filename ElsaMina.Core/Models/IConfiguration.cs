namespace ElsaMina.Core.Models;

public interface IConfiguration
{
    string? Env { get; }
    string? Host { get; }
    string? Port { get; }
    string? Name { get; }
    string? Password { get; }
    IEnumerable<string>? Rooms { get; }
    IEnumerable<string>? RoomBlacklist { get; }
}