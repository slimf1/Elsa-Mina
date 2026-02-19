using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Users;

[NamedCommand("alts")]
public class AltsCommand : Command
{
    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}