using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Development;

[NamedCommand("stop-connection")]
public class StopConnection : DevelopmentCommand
{
    private readonly IClient _client;

    public StopConnection(IClient client)
    {
        _client = client;
    }

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        Log.Information("Stopping connection : {0}", context);
        await _client.Close();
    }
}