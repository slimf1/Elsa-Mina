using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.System;
using Serilog;

namespace ElsaMina.Commands.Development;

public class Kill : Command<Help>, INamed
{
    public static string Name => "kill";
    public override bool IsWhitelistOnly => true;
    public override bool IsHidden => true;
    public override bool IsAllowedInPm => true;

    private readonly ILogger _logger;
    private readonly ISystemService _systemService;

    public Kill(ILogger logger, ISystemService systemService)
    {
        _logger = logger;
        _systemService = systemService;
    }

    public override Task Run(IContext context)
    {
        _logger.Information("Killing bot : {0}", context);
        _systemService.Kill();
        return Task.CompletedTask;
    }
}