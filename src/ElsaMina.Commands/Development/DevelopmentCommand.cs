using ElsaMina.Core.Commands;

namespace ElsaMina.Commands.Development;

public abstract class DevelopmentCommand : Command
{
    public override bool IsAllowedInPm => true;
    public override bool IsWhitelistOnly => true;
    public override bool IsHidden => true;
}