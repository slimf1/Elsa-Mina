using ElsaMina.Core.Client;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using Serilog;

namespace ElsaMina.Commands.Development;

public class StopConnection : ICommand
{
    public static string Name => "stop-connection";
    public bool IsAllowedInPm => true;
    public bool IsWhitelistOnly => true;
    public bool IsHidden => true;

    private readonly IClient _client;
    private readonly ILogger _logger;

    public StopConnection(IClient client,
        ILogger logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task Run(IContext context)
    {
        _logger.Information("Stopping connection : {0}", context);
        await _client.Close();
    }
}