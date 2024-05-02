using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using Serilog;

namespace ElsaMina.Commands.Development;

public class StopConnection : Command<StopConnection>, INamed
{
    public static string Name => "stop-connection";

    private readonly IClient _client;
    private readonly ILogger _logger;

    public StopConnection(IClient client,
        ILogger logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public override bool IsAllowedInPm => true;
    public override bool IsWhitelistOnly => true;
    public override bool IsHidden => true;

    public override async Task Run(IContext context)
    {
        _logger.Information("Stopping connection : {0}", context);
        await _client.Close();
    }
}