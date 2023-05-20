using ElsaMina.Core.Models;

namespace ElsaMina.Core.Commands;

public abstract class Parser : IParser
{
    public bool IsEnabled { get; set; } = true;

    public abstract Task Execute(string[] parts);
}