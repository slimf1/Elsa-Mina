using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands;

public abstract class Command : ICommand
{
    protected Command()
    {
        InitializeNameAndAliases();
    }

    public string CommandName { get; private set; } = string.Empty;
    public IEnumerable<string> CommandAliases { get; private set; } = Enumerable.Empty<string>();
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

    private void InitializeNameAndAliases()
    {
        if (GetType().GetCustomAttributes(typeof(NamedCommandAttribute), false).FirstOrDefault()
            is not NamedCommandAttribute commandAttribute)
        {
            return;
        }

        CommandName = commandAttribute.Name;
        CommandAliases = commandAttribute.Aliases;
    }
}