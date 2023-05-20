namespace ElsaMina.Core.Models;

public interface IParser
{
    bool IsEnabled { get; set; }
    Task Execute(string[] parts);
}