using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Development;

[NamedCommand("stop-connection")]
public class StopConnectionCommand : DevelopmentCommand
{
    private readonly IClient _client;

    public StopConnectionCommand(IClient client)
    {
        _client = client;
    }

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        Log.Information("Stopping connection : {0}", context);
        await _client.Close();
    }
}