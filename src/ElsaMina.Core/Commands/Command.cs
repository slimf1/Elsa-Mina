using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Commands;

public abstract class Command : ICommand
{
    protected Command()
    {
        InitializeNameAndAliasesFromAttribute();
    }

    public string CommandName { get; private set; }
    public IEnumerable<string> CommandAliases { get; private set; }
    public virtual bool IsAllowedInPrivateMessage => false;
    public virtual bool IsWhitelistOnly => false;
    public virtual bool IsPrivateMessageOnly => false;
    public virtual char RequiredRank => '~';
    public virtual string HelpMessageKey => string.Empty;
    public virtual bool IsHidden => false;
    public virtual string[] AllowedRooms => [];

    public void ReplyLocalizedHelpMessage(IContext context, params object[] formatArguments)
    {
        context.Reply(context.GetString(HelpMessageKey, formatArguments));
    }

    public virtual Task OnBotStartUp()
    {
        return Task.CompletedTask;
    }

    public async Task Call(IContext context)
    {
        if (IsPrivateMessageOnly && !context.IsPrivateMessage)
        {
            return;
        }

        if (context.IsPrivateMessage && !(IsAllowedInPrivateMessage || IsPrivateMessageOnly))
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

        if (AllowedRooms.Length > 0 && !AllowedRooms.Contains(context.RoomId))
        {
            return;
        }

        await Run(context);
    }

    public abstract Task Run(IContext context);

    private void InitializeNameAndAliasesFromAttribute()
    {
        var commandAttribute = GetType().GetCommandAttribute();
        CommandName = commandAttribute?.Name ?? string.Empty;
        CommandAliases = commandAttribute?.Aliases ?? [];
    }
}