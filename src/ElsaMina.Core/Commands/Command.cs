using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands;

public abstract class Command<T> : ICommand where T : INamed
{
    public string CommandName => T.Name;
    public IEnumerable<string> CommandAliases => T.Aliases;
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