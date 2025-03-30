using ElsaMina.Core.Services.System;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public sealed class NameTakenHandler : Handler
{
    private readonly ISystemService _systemService;

    public NameTakenHandler(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        if (parts.Length >= 2 && parts[1] == "nametaken")
        {
            Log.Error("Login failed, check username or password validity. Exiting");
            _systemService.Kill();
        }

        return Task.CompletedTask;
    }
}