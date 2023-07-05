using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands;

public abstract class BaseCommand<T> : ICommand where T : ICommand
{
    public virtual string CommandName => T.Name;
    public virtual IEnumerable<string> CommandAliases => T.Aliases;
    public virtual bool IsAllowedInPm => false;
    public virtual bool IsWhitelistOnly => false;
    public virtual bool IsPrivateMessageOnly => false;
    public virtual char RequiredRank => '&';
    public virtual string HelpMessageKey => string.Empty;
    public virtual bool IsHidden => false;

    public void ReplyLocalizedHelpMessage(IContext context, params object[] formatArguments)
    {
        context.Reply(context.GetString(HelpMessageKey, formatArguments));
    }

    public async Task Call(IContext context)
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

    public abstract Task Run(IContext context);
}