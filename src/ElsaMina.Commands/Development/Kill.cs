using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using Serilog;

namespace ElsaMina.Commands.Development;

public class Kill : BaseCommand<Help>, INamed
{
    public static string Name => "kill";
    public override bool IsWhitelistOnly => true;
    public override bool IsHidden => true;
    public override bool IsAllowedInPm => true;

    private readonly ILogger _logger;

    public Kill(ILogger logger)
    {
        _logger = logger;
    }

    public override Task Run(IContext context)
    {
        _logger.Information("Killing bot : {0}", context);
        Environment.Exit(1);
        return Task.CompletedTask;
    }
}