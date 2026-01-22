using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Development;

[NamedCommand("fail", "error", "throw")]
public class FailCommand : DevelopmentCommand
{
    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new ArgumentException("fail command called");
    }
}