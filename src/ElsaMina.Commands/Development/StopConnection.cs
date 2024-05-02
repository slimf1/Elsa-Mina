using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class StopConnection : Command<StopConnection>, INamed
{
    public static string Name => "stop-connection";

    private readonly IClient _client;

    public StopConnection(IClient client)
    {
        _client = client;
    }
    
    public override bool IsAllowedInPm => true;
    public override bool IsWhitelistOnly => true;
    public override bool IsHidden => true;

    public override async Task Run(IContext context)
    {
        Logger.Current.Information("Stopping connection : {0}", context);
        await _client.Close();
    }
}