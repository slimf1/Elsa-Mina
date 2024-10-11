namespace ElsaMina.Core.Models;

public class Configuration : IConfiguration
{
    public string Env { get; set; }
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
}