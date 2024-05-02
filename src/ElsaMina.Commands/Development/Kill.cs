using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.System;

namespace ElsaMina.Commands.Development;

public class Kill : Command<Help>, INamed
{
    public static string Name => "kill";
    public override bool IsWhitelistOnly => true;
    public override bool IsHidden => true;
    public override bool IsAllowedInPm => true;

    private readonly ISystemService _systemService;

    public Kill(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public override Task Run(IContext context)
    {
        Logger.Current.Information("Killing bot : {0}", context);
        _systemService.Kill();
        return Task.CompletedTask;
    }
}