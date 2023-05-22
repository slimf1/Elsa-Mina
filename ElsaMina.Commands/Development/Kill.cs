using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using Serilog;

namespace ElsaMina.Commands.Development;

public class Kill : ICommand
{
    public static string Name => "kill";
    public bool IsWhitelistOnly => true;
    public bool IsHidden => true;
    public bool IsAllowedInPm => true;

    private readonly ILogger _logger;

    public Kill(ILogger logger)
    {
        _logger = logger;
    }

    public Task Run(IContext context)
    {
        _logger.Information("Killing bot : {0}", context);
        Environment.Exit(1);
        return Task.CompletedTask;
    }
}