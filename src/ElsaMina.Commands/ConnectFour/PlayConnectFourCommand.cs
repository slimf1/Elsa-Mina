using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.ConnectFour;

[NamedCommand("c4play")]
public class PlayConnectFourCommand : Command
{
    public override bool IsPrivateMessageOnly => true;
    public override bool IsAllowedInPrivateMessage => true;

    public override Task Run(IContext context)
    {
        // TODO
        return Task.CompletedTask;
    }
}