using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.System;

namespace ElsaMina.Commands.Development;

[NamedCommand("kill")]
public class Kill : DevelopmentCommand
{
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