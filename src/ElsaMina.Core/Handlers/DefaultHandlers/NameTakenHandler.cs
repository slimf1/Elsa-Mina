using ElsaMina.Core.Services.System;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public sealed class NameTakenHandler : Handler
{
    private readonly ISystemService _systemService;

    public NameTakenHandler(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public override string Identifier => nameof(NameTakenHandler);

    protected override Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (parts.Length >= 2 && parts[1] == "nametaken")
        {
            Logger.Current.Error("Login failed, check username or password validity. Exiting");
            _systemService.Kill();
        }
        
        return Task.CompletedTask;
    }
}