using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

[NamedCommand("fail", "error", "throw")]
public class FailCommand : DevelopmentCommand
{
    public override Task Run(IContext context)
    {
        throw new Exception("fail command called");
    }
}