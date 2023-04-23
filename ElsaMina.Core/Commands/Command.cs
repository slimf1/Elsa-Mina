using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands;

public abstract class Command
{
    public abstract string Name { get; }
    public virtual IEnumerable<string> Aliases { get; protected set; } = Enumerable.Empty<string>();
    protected virtual bool IsAllowedInPm => false;
    protected virtual bool IsWhitelistOnly => false;
    protected virtual bool IsPrivateMessageOnly => false;
    protected virtual char RequiredRank => '&';
    protected virtual string HelpMessage => "";
    protected virtual bool IsHidden => false;

    public async Task Call(Context context)
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

    protected abstract Task Run(Context context);
}