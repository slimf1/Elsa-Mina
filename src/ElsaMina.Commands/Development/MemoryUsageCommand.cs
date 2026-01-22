using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.System;

namespace ElsaMina.Commands.Development;

[NamedCommand("memusage", "memoryusage")]
public class MemoryUsageCommand : Command
{
    private readonly ISystemService _systemService;

    public MemoryUsageCommand(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var systemInfo = _systemService.GetSystemInfo();
        context.Reply($"!code {systemInfo}");

        return Task.CompletedTask;
    }
}