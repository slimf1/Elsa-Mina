namespace ElsaMina.Core.Models;

public interface IConfiguration
{
    public string? Env { get; }
    public string? Host { get; }
    string? Port { get; }
    string? Name { get; }
    string? Password { get; }
    IEnumerable<string>? Rooms { get; }
}