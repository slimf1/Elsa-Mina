using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands;

public interface ICommand
{
    public static virtual string Name => string.Empty;
    public static virtual IEnumerable<string> Aliases => Enumerable.Empty<string>();
    protected virtual bool IsAllowedInPm => false;
    protected virtual bool IsWhitelistOnly => false;
    protected virtual bool IsPrivateMessageOnly => false;
    protected virtual char RequiredRank => '&';
    protected virtual string HelpMessage => "";
    protected virtual bool IsHidden => false;

    public sealed async Task Call(IContext context)
    {
        if (IsPrivateMessageOnly && !context.IsPm)
        {
            return;
        }

        if (context.IsPm && !(IsAllowedInPm || IsPrivateMessageOnly))
        {
            return;
        }

        if (IsWhitelistOnly && !context.IsSenderWhitelisted)
        {
            return;
        }

        if (!context.HasSufficientRank(RequiredRank))
        {
            return;
        }

        await Run(context);
    }

    protected Task Run(IContext context);
}