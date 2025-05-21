using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

[NamedCommand("fail", "error", "throw")]
public class FailCommand : DevelopmentCommand
{
    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new ArgumentException("fail command called");
    }
}