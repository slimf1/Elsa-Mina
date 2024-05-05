using ElsaMina.Core.Services.System;

namespace ElsaMina.Core.Parsers.DefaultParsers;

public sealed class NameTakenParser : Parser
{
    private readonly ISystemService _systemService;

    public NameTakenParser(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public override string Identifier => nameof(NameTakenParser);

    protected override Task Execute(string[] parts, string roomId = null)
    {
        if (parts.Length >= 2 && parts[1] == "nametaken")
        {
            Logger.Current.Error("Login failed, check username or password validity. Exiting");
            _systemService.Kill();
        }
        
        return Task.CompletedTask;
    }
}