namespace ElsaMina.Core.Models;

public class Configuration : IConfiguration
{
    public string? Env { get; set; }
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public IEnumerable<string>? Rooms { get; set; }
    public IEnumerable<string>? RoomBlacklist { get; }
}